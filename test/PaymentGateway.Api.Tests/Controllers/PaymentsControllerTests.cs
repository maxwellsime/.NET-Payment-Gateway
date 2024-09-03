using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq.Protected;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private static readonly PaymentsRepository PaymentsRepository = new();
    private static readonly Mock<HttpMessageHandler> MockHttpHandler = new(MockBehavior.Strict);
    private static readonly HttpClient MockHttpClient = new(MockHttpHandler.Object);
    private static readonly BankService BankService = new(PaymentGatewayTestFixtures.BankURL, MockHttpClient);
    private static readonly WebApplicationFactory<PaymentsController> WebApplicationFactory = new();
    private readonly HttpClient _client = WebApplicationFactory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services => ((ServiceCollection)services)
            .AddSingleton(PaymentsRepository)
            .AddSingleton(_ => BankService)))
        .CreateClient();

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var expectedResponse = PaymentsRepository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Authorized);

        // Act
        var response = await _client.GetAsync($"/api/Payments/{expectedResponse.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse?>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/Payments/{PaymentGatewayTestFixtures.InvalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CompletesAnAuthorizedPayment()
    {
        // Arrange
        var expectedResponse = PaymentGatewayTestFixtures.PaymentRequest.ToPostPaymentResponse(PaymentStatus.Authorized, PaymentGatewayTestFixtures.Id);
        MockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(PaymentGatewayTestFixtures.ValidBankResponse), Encoding.UTF8, "application/json")
            })
            .Verifiable();

        // Act
        var response = await _client.PostAsync($"/api/Payments/MakePayment", JsonContent.Create(PaymentGatewayTestFixtures.PaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse.Status, paymentResponse!.Status);
        MockHttpHandler.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Returns400IfPaymentDeclined()
    {
        // Arrange
        MockHttpHandler.Invocations.Clear();
        var expectedResponse = PaymentGatewayTestFixtures.PaymentRequest.ToPostPaymentResponse(PaymentStatus.Declined, PaymentGatewayTestFixtures.Id);
        MockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(PaymentGatewayTestFixtures.InvalidBankResponse), Encoding.UTF8, "application/json")
            })
            .Verifiable();

        // Act
        var response = await _client.PostAsync($"/api/Payments/MakePayment", JsonContent.Create(PaymentGatewayTestFixtures.PaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(expectedResponse.Status, paymentResponse!.Status);
        MockHttpHandler.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Returns400IfPaymentRequestRejected()
    {
        // Arrange
        var expectedResponse = PaymentGatewayTestFixtures.PaymentRequest.ToPostPaymentResponse(PaymentStatus.Rejected, null, ["error"]);

        // Act
        var response = await _client.PostAsync($"/api/Payments/MakePayment", JsonContent.Create(PaymentGatewayTestFixtures.RejectedPaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(expectedResponse.Status, paymentResponse!.Status);
        Assert.NotNull(paymentResponse.Errors);
    }
}