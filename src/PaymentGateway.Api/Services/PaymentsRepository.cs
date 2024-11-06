using MongoDB.Driver;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    Task<Payment> Add(PostPaymentRequest paymentRequest, PaymentStatus status);
    Task<Payment?> GetById(string id);
    Task<List<Payment>> GetByCardNumber(string cardNumberLastFour);
}

public class PaymentsRepository : IPaymentsRepository
{
    private readonly IMongoCollection<Payment>? _table;

    public PaymentsRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
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

    public async Task<Payment?> GetById(string id)
    {
        Console.WriteLine("PaymentsRepository :: Getting payment.");

        var filter = Builders<Payment>.Filter.Eq(x => x.Id, id);
        try
        {
            var payment = await _table.Find(filter).SingleAsync();
            return payment is not null ? payment : null;
        }
        catch (FormatException)
        {
            Console.WriteLine($"PaymentsRepository :: Payment {id} does not exist.");

            return null;
        }
    }

    public async Task<List<Payment>> GetByCardNumber(string cardNumberLastFour)
    {
        Console.WriteLine("PaymentsRepository :: Getting payment by card number.");

        var filter = Builders<Payment>.Filter.Eq(x => x.CardNumberLastFour, cardNumberLastFour);
        try
        {
            var payments = await _table.Find(filter).ToListAsync();
            return payments;
        }
        catch (FormatException)
        {
            return [];
        }
    }
}