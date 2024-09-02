using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly PaymentsRepository _paymentsRepository;
    private readonly BankService _bankService;

    public PaymentsController(PaymentsRepository paymentsRepository, BankService bankService)
    {
        _paymentsRepository = paymentsRepository;
        _bankService = bankService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        Console.WriteLine($"PaymentsController :: Getting payment history of id {id}.");

        var payment = _paymentsRepository.Get(id);

        return payment != null
            ? new OkObjectResult(payment)
            : new NotFoundObjectResult($"No payment with id: {id} found.");
    }

    [HttpPost("MakePayment")]
    public async Task<ActionResult<PostPaymentResponse>> CreatePaymentAsync(PostPaymentRequest request)
    {
        Console.WriteLine("PaymentsController :: Making payment.");

        if (this.ModelState.IsValid)
        {
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
        else
        {
            Console.WriteLine("PaymentsController :: Request invalid causing rejection.");

            var errors = ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(request.ToPostPaymentResponse(PaymentStatus.Rejected, null, errors));
        }
    }
}