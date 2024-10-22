using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IBankService
{
    Task<bool> MakePaymentAsync(BankPaymentRequest request);
}

public class BankService : IBankService
{
    private readonly string _bankURL;
    private readonly HttpClient _httpClient;

    public BankService(IConfiguration configuration, HttpClient? client) 
    {
        _bankURL = configuration.GetConnectionString("BankService")!;
        _httpClient = client ?? new();
    }

    public async Task<bool> MakePaymentAsync(BankPaymentRequest request)
    {
        try
        {
            Console.WriteLine("BankService :: Contacting bank for payment.");

            JsonContent content = JsonContent.Create(request);
            var httpResponse = await _httpClient.PostAsync($"{_bankURL}/payments", content);

            return httpResponse.Content.ReadFromJsonAsync<BankResponse>().Result.Authorized;
        }
        catch (Exception)
        {
            Console.WriteLine("BankService :: Making payment to bank responded with an error.");

            throw;
        }
    }
}