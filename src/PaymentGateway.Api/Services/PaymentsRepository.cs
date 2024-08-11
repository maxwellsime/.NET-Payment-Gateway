using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    private readonly List<PostPaymentResponse> _payments = [];

    public PostPaymentResponse Add(PostPaymentRequest payment, PaymentStatus status)
    {
        Console.WriteLine("PaymentsRepository :: Adding payment.");

        var postPaymentResponse = payment.ToPostPaymentResponse(status, Guid.NewGuid());
        _payments.Add(postPaymentResponse);

        return postPaymentResponse;
    }

    public PostPaymentResponse? Get(Guid id)
    {
        Console.WriteLine("PaymentsRepository :: Getting payment.");

        return _payments.Find(p => p.Id == id);
    }
}