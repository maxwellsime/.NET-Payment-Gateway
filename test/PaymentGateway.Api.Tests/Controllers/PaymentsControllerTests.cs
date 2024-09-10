using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private static readonly Mock<IPaymentsRepository> PaymentsRepository = new();
    private static readonly Mock<IBankService> BankService = new();

    public static HttpClient TestHttpClientFactory() =>
        new WebApplicationFactory<PaymentsController>().WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => PaymentsRepository.Object);
                services.AddScoped(_ => BankService.Object);
            })).CreateClient();

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        PaymentsRepository
            .Setup(repository => repository.Get(It.IsAny<Guid>()))
            .Returns(PaymentGatewayTestFixtures.PaymentRequest.ToPostPaymentResponse(PaymentStatus.Authorized, Guid.NewGuid()))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.GetAsync($"/api/Payments/{PaymentGatewayTestFixtures.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse?>();

        // Assert
        PaymentsRepository.Verify(repository => repository.Get(PaymentGatewayTestFixtures.Id), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var client = TestHttpClientFactory();

        // Act
        var response = await client.GetAsync($"/api/Payments/{PaymentGatewayTestFixtures.InvalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CompletesAnAuthorizedPayment()
    {
        // Arrange
        PaymentsRepository.Invocations.Clear();
        BankService
            .Setup(bank => bank.MakePaymentAsync(PaymentGatewayTestFixtures.PaymentRequest.ToBankPaymentRequest()).Result)
            .Returns(true)
            .Verifiable();
        PaymentsRepository
            .Setup(repository => repository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Authorized))
            .Returns(PaymentGatewayTestFixtures.PaymentRequest.ToPostPaymentResponse(PaymentStatus.Authorized, Guid.NewGuid()))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.PostAsync($"/api/Payments/MakePayment", JsonContent.Create(PaymentGatewayTestFixtures.PaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        BankService.Verify(bank => bank.MakePaymentAsync(PaymentGatewayTestFixtures.PaymentRequest.ToBankPaymentRequest()), Times.Exactly(1));
        PaymentsRepository.Verify(repository => repository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Authorized), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse!.Status);
    }

    [Fact]
    public async Task Returns400IfPaymentDeclined()
    {
        // Arrange
        BankService.Invocations.Clear();
        PaymentsRepository.Invocations.Clear();
        BankService
            .Setup(bank => bank.MakePaymentAsync(It.IsAny<BankPaymentRequest>()))
            .Returns(Task.FromResult(false))
            .Verifiable();
        PaymentsRepository
            .Setup(repository => repository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Declined))
            .Returns(PaymentGatewayTestFixtures.PaymentRequest.ToPostPaymentResponse(PaymentStatus.Declined, Guid.NewGuid()))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.PostAsync($"/api/Payments/MakePayment", JsonContent.Create(PaymentGatewayTestFixtures.PaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        BankService.Verify(bank => bank.MakePaymentAsync(PaymentGatewayTestFixtures.PaymentRequest.ToBankPaymentRequest()), Times.Exactly(1));
        PaymentsRepository.Verify(repository => repository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Declined), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(PaymentStatus.Declined, paymentResponse!.Status);
    }

    [Fact]
    public async Task Returns400IfPaymentRequestRejected()
    {
        // Arrange
        var client = TestHttpClientFactory();

        // Act
        var response = await client.PostAsync($"/api/Payments/MakePayment", JsonContent.Create(PaymentGatewayTestFixtures.RejectedPaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(PaymentStatus.Rejected, paymentResponse!.Status);
        Assert.NotNull(paymentResponse.Errors);
    }
}