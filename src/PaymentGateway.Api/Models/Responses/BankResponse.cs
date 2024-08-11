using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses;

public class BankResponse(bool authorized, string authorizationCode = "")
{
    public bool Authorized { get; set; } = authorized;

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = authorizationCode;
}
