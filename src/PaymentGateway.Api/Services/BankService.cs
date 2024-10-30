using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IBankService
{
    Task<bool> MakePaymentAsync(BankPaymentRequest request);
}

public class BankService(HttpClient client) : IBankService
{
    private readonly HttpClient _httpClient = client;

    public async Task<bool> MakePaymentAsync(BankPaymentRequest request)
    {
        try
        {
            Console.WriteLine("BankService :: Contacting bank for payment.");

            var httpResponse = await _httpClient.PostAsJsonAsync("payments", request);

            return httpResponse.Content.ReadFromJsonAsync<BankResponse>().Result.Authorized;
        }
        catch (Exception)
        {
            Console.WriteLine("BankService :: Making payment to bank responded with an error.");

            throw;
        }
    }
}