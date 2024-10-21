using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    [JsonPropertyName("_id")]
    public Guid? Id { get; set; }

    [Required]
    [JsonPropertyName("status")]
    public PaymentStatus Status { get; set; }

    [Required]
    [JsonPropertyName("cardNumberLastFour")]
    public string CardNumberLastFour { get; set; } = "";

    [Required]
    [JsonPropertyName("expiryMonth")]
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    [Required]
    [JsonPropertyName("expiryYear")]
    public int ExpiryYear { get; set; }

    [Required]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "";

    [Required]
    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("errors")]
    public List<string>? Errors { get; set; }
}