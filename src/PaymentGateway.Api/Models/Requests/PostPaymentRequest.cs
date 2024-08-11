using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest(string cardNumber, int expiryMonth, int expiryYear, string currency, int amount, int cvv)
{
    [Required]
    [RegularExpression("[0-9]{14,19}", ErrorMessage = "CardNumber only accepts integers between 14 and 19.")]
    public string CardNumber { get; } = cardNumber;

    [Required]
    [Range(1, 12, ErrorMessage = "ExpiryMonth only accepts integers between 1 and 12.")]
    public int ExpiryMonth { get; } = expiryMonth;

    [Required]
    [YearFromNow("ExpiryYear needs to be beyond the current year.")]
    public int ExpiryYear { get; } = expiryYear;

    [Required]
    [RegularExpression("^(GBP|USD|EUR)", ErrorMessage = "Currency only accepts 3 character long strings.")]
    public string Currency { get; } = currency;

    [Required]
    public int Amount { get; } = amount;

    [Required]
    [Range(000, 999, ErrorMessage = "Cvv only accepts 3 character long strings.")]
    public int Cvv { get; } = cvv;

    public PostPaymentResponse ToPostPaymentResponse(PaymentStatus status, Guid? id = null, List<string>? errors = null)
    {
        PostPaymentResponse response = new()
        {
            Id = id,
            Status = status,
            Amount = Amount,
            CardNumberLastFour = (CardNumber.Length) - 4 > 0 ? CardNumber[^4..] : CardNumber,
            Currency = Currency,
            Errors = errors,
            ExpiryMonth = ExpiryMonth,
            ExpiryYear = ExpiryYear,
        };

        return response;
    }

    public BankPaymentRequest ToBankPaymentRequest() => new(this);
}