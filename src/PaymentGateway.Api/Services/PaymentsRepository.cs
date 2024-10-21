using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using MongoDB.Driver;
using PaymentGateway.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    Task<Payment> Add(PostPaymentRequest payment, PaymentStatus status);
    Task<Payment?> Get(string id);
}

public class PaymentsRepository : IPaymentsRepository
{
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<Payment>? _table;

    public PaymentsRepository(IConfiguration configuration) {
        _configuration = configuration;

        var connectionString = _configuration.GetConnectionString("DbConnection");
        var mongoUrl = MongoUrl.Create(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        _table = database.GetCollection<Payment>("payments");
    }

    public async Task<Payment> Add(PostPaymentRequest paymentRequest, PaymentStatus status)
    {
        Console.WriteLine("PaymentsRepository :: Adding payment.");

        var paymentEntity = paymentRequest.ToEntity(status);
        await _table!.InsertOneAsync(paymentEntity);
        return paymentEntity;
    }

    public async Task<Payment?> Get(string id)
    {
        Console.WriteLine("PaymentsRepository :: Getting payment.");

        var filter = Builders<Payment>.Filter.Eq(x => x.Id, id);
        var payment = await _table.Find(filter).SingleAsync();
        return payment is not null ? payment : null;
    }
}