using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class BankPaymentRequest(PostPaymentRequest postPaymentRequest)
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; } = postPaymentRequest.CardNumber;

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; } = $"""
        {(postPaymentRequest.ExpiryMonth.ToString().Length == 1 ? "0" + postPaymentRequest.ExpiryMonth : postPaymentRequest.ExpiryMonth.ToString())}/{postPaymentRequest.ExpiryYear}
        """;

    public string Currency { get; } = postPaymentRequest.Currency;

    public int Amount { get; } = postPaymentRequest.Amount;

    public string Cvv { get; } = postPaymentRequest.Cvv.ToString();
}