using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsRepository paymentsRepository, IBankService bankService) : ControllerBase
{
    private readonly IPaymentsRepository _paymentsRepository = paymentsRepository;
    private readonly IBankService _bankService = bankService;

    [HttpGet("history-id/{id}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(string id)
    {
        Console.WriteLine($"PaymentsController :: Getting payment history of id {id}.");

        var payment = await _paymentsRepository.GetById(id);

        return payment != null
            ? new OkObjectResult(payment)
            : new NotFoundObjectResult($"No payment with id: {id} found.");
    }

    [HttpGet("history-cardnumberlastfour/{cardNumberLastFour}")]
    public async Task<ActionResult<List<PostPaymentResponse>>> GetPaymentsHistoryAsync(string cardNumberLastFour) 
    {
        Console.WriteLine($"PaymentsController :: Getting payment history for card ending in {cardNumberLastFour}.");

        var payments = await _paymentsRepository.GetByCardNumber(cardNumberLastFour);

        return payments.Count > 0
            ? new OkObjectResult(payments)
            : new NotFoundObjectResult($"No payments found for card ending in {cardNumberLastFour}.");
    }

    [HttpPost("create-payment")]
    public async Task<ActionResult<PostPaymentResponse>> CreatePaymentAsync(PostPaymentRequest request)
    {
        Console.WriteLine("PaymentsController :: Making payment.");

        try
        {
            var paymentResponse = await _bankService.MakePaymentAsync(request.ToBankPaymentRequest());

            return paymentResponse
                ? new OkObjectResult(_paymentsRepository.Add(request, PaymentStatus.Authorized))
                : new BadRequestObjectResult(_paymentsRepository.Add(request, PaymentStatus.Declined));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PaymentsController :: Request caught exception {ex.Message}");

            return StatusCode(500, ex);
        }
    }
}