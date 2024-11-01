using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public static class PaymentGatewayTestFixtures
{
    public static readonly string Id = "1dd16ebe-cf73-4f2b-8d88-5114b669ebee";
    public static readonly string InvalidId = "4a89ad4a-3e17-46b8-bab0-ea7683c0ede1";
    public static readonly string CardNumber = "123456789012345";
    public static readonly int ExpiryMonth = 4;
    public static readonly int ExpiryYear = 2024;
    public static readonly int Amount = 100;
    public static readonly string Currency = "GBP";
    public static readonly int Cvv = 123;
    public static readonly string BankURL = "localhost:8080";

    public static readonly PostPaymentRequest PaymentRequest = new(CardNumber, ExpiryMonth, ExpiryYear, Currency, Amount, Cvv);
    public static readonly PostPaymentRequest RejectedPaymentRequest = new("12345", ExpiryMonth, 2023, Currency, Amount, 111111);

    public static readonly BankResponse ValidBankResponse = new(true, "");
    public static readonly BankResponse InvalidBankResponse = new(false, "");
}