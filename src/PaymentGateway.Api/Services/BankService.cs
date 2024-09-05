using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IBankService {
    Task<bool> MakePaymentAsync(BankPaymentRequest request);
}

public class BankService(string bankURL, HttpClient client) : IBankService
{
    private readonly string _bankURL = bankURL;
    private readonly HttpClient _httpClient = client;

    public async Task<bool> MakePaymentAsync(BankPaymentRequest request)
    {
        try
        {
            Console.WriteLine("BankService :: Contacting bank for payment.");

            JsonContent content = JsonContent.Create(request);
            var httpResponse = await _httpClient.PostAsync($"{_bankURL}/payments", content);

            return httpResponse.Content.ReadFromJsonAsync<BankResponse>().Result.Authorized;
        }
        catch (Exception) {
            Console.WriteLine("BankService :: Making payment to bank responded with an error.");

            throw;
        }
    }
}