using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models.Entities;

public class Payment
{
    [BsonId]
    [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("payment_date"), BsonRepresentation(BsonType.DateTime)]
    public required DateTime PaymentDate { get; set; }

    [BsonElement("status"), BsonRepresentation(BsonType.String)]
    public required PaymentStatus Status { get; set; }

    [BsonElement("card_number_last_four"), BsonRepresentation(BsonType.String)]
    public required string CardNumberLastFour { get; set; }

    [BsonElement("expiration_date"), BsonRepresentation(BsonType.DateTime)]
    public DateOnly ExpirationDate { get; set; }

    [BsonElement("currency"), BsonRepresentation(BsonType.String)]
    public required string Currency { get; set; }

    [BsonElement("amount"), BsonRepresentation(BsonType.Int64)]
    public int Amount { get; set; }

    public PostPaymentResponse ToPaymentResponse => new PostPaymentResponse()
    {
        Id = Id,
        Status = Status,
        Amount = Amount,
        CardNumberLastFour = CardNumberLastFour,
        Currency = Currency,
        ExpiryMonth = ExpirationDate.Month,
        ExpiryYear = ExpirationDate.Day
    };
}