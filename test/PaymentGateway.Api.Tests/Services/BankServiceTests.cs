using System.Net;
using System.Text;
using System.Text.Json;

using Moq.Protected;

using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

public class BankServiceTests
{
    private static readonly Mock<HttpMessageHandler> MockHttpHandler = new(MockBehavior.Strict);
    private static readonly HttpClient MockHttpClient = new(MockHttpHandler.Object) { BaseAddress = new Uri("http://localhost:80/") };
    private static readonly BankService BankService = new(MockHttpClient);

    [Fact]
    public async Task RespondTrueOnAuthorizedPayment()
    {       
        // Arrange
        MockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(PaymentGatewayTestFixtures.ValidBankResponse), Encoding.UTF8, "application/json")
            })
            .Verifiable();

        // Act
        var response = await BankService.MakePaymentAsync(PaymentGatewayTestFixtures.PaymentRequest.ToBankPaymentRequest());

        // Assert
        Assert.Equal(response, PaymentGatewayTestFixtures.ValidBankResponse.Authorized);
        MockHttpHandler.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task RespondFalseOnUnauthorizedPayment()
    {
        // Arrange
        MockHttpHandler.Invocations.Clear();
        MockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(PaymentGatewayTestFixtures.InvalidBankResponse), Encoding.UTF8, "application/json")
            })
            .Verifiable();

        // Act
        var response = await BankService.MakePaymentAsync(PaymentGatewayTestFixtures.RejectedPaymentRequest.ToBankPaymentRequest());

        // Assert
        Assert.Equal(response, PaymentGatewayTestFixtures.InvalidBankResponse.Authorized);
        MockHttpHandler.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task ThrowExceptionOnConnectionFailure()
    {
        // Arrange
        MockHttpHandler.Invocations.Clear();
        MockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Throws<Exception>()
            .Verifiable();

        // Act
        await Assert.ThrowsAsync<Exception>(() => BankService.MakePaymentAsync(PaymentGatewayTestFixtures.PaymentRequest.ToBankPaymentRequest()));
        MockHttpHandler.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}