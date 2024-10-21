namespace PaymentGateway.Api.Models.Entities;

using PaymentGateway.Api.Enums;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Payment {
    [BsonId]
    [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
    public int? Id { get; set; }

    [BsonElement("payment_date"), BsonRepresentation(BsonType.DateTime)]
    public DateTime PaymentDate { get; set; }

    [BsonElement("status"), BsonRepresentation(BsonType.String)]
    public PaymentStatus? Status { get; set; }

    [BsonElement("card_number_last_four"), BsonRepresentation(BsonType.String)]
    public string? CardNumberLastFour { get; set; }

    [BsonElement("expiration_date"), BsonRepresentation(BsonType.DateTime)]
    public DateOnly? ExpirationDate { get; set; }

    [BsonElement("currency"), BsonRepresentation(BsonType.String)]
    public string? Currency { get; set; }

    [BsonElement("amount"), BsonRepresentation(BsonType.Int64)]
    public int? Amount { get; set; }
}