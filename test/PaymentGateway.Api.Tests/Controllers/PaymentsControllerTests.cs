﻿using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private static readonly Mock<IPaymentsRepository> PaymentsRepository = new();
    private static readonly Mock<IBankService> BankService = new();
    private static readonly HttpClient DefaultClient = TestHttpClientFactory();

    public static HttpClient TestHttpClientFactory() =>
        new WebApplicationFactory<PaymentsController>().WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => PaymentsRepository.Object);
                services.AddScoped(_ => BankService.Object);
            })).CreateClient();

    [Fact]
    public async Task GetAPaymentSuccessfullyById()
    {
        // Arrange
        PaymentsRepository
            .Setup(repository => repository.GetById(It.IsAny<String>()))
            .Returns(Task.FromResult(PaymentGatewayTestFixtures.PaymentRequest.ToEntity(PaymentStatus.Authorized)))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.GetAsync($"/api/payments/{PaymentGatewayTestFixtures.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse?>();

        // Assert
        PaymentsRepository.Verify(repository => repository.GetById(PaymentGatewayTestFixtures.Id), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task GetByIdReturnsNotFound()
    {
        // Act
        var response = await DefaultClient.GetAsync($"/api/payments/{PaymentGatewayTestFixtures.InvalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaymentHistorySuccessfully()
    {
        // Arrange
        PaymentsRepository
            .Setup(repository => repository.GetByCardNumber(It.IsAny<String>()))
            .Returns(Task.FromResult(new List<Payment> { PaymentGatewayTestFixtures.PaymentRequest.ToEntity(PaymentStatus.Authorized) }))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.GetAsync($"/api/payments/multiple/{PaymentGatewayTestFixtures.CardNumber}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<List<PostPaymentResponse?>>();

        // Assert
        PaymentsRepository.Verify(repository => repository.GetById(PaymentGatewayTestFixtures.Id), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task GetPaymentsHistoryReturnsNotFound()
    {
        // Arrange
        PaymentsRepository
            .Setup(repository => repository.GetByCardNumber(It.IsAny<String>()))
            .Returns(Task.FromResult(new List<Payment>()))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.GetAsync($"/api/payments/multiple/{PaymentGatewayTestFixtures.InvalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task CreateAnAuthorizedPayment()
    {
        // Arrange
        BankService.Invocations.Clear();
        PaymentsRepository.Invocations.Clear();
        BankService
            .Setup(bank => bank.MakePaymentAsync(It.IsAny<BankPaymentRequest>()))
            .Returns(Task.FromResult(true))
            .Verifiable();
        PaymentsRepository
            .Setup(repository => repository.Add(It.IsAny<PostPaymentRequest>(), PaymentStatus.Authorized))
            .Returns(Task.FromResult(PaymentGatewayTestFixtures.PaymentRequest.ToEntity(PaymentStatus.Authorized)))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.PostAsync($"/api/payments/create-payment", JsonContent.Create(PaymentGatewayTestFixtures.PaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        BankService.Verify(bank => bank.MakePaymentAsync(It.IsAny<BankPaymentRequest>()), Times.Exactly(1));
        PaymentsRepository.Verify(repository => repository.Add(It.IsAny<PostPaymentRequest>(), PaymentStatus.Authorized), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse!.Status);
    }

    [Fact]
    public async Task CreatePaymentReturns400IfPaymentDeclined()
    {
        // Arrange
        BankService.Invocations.Clear();
        PaymentsRepository.Invocations.Clear();
        BankService
            .Setup(bank => bank.MakePaymentAsync(It.IsAny<BankPaymentRequest>()))
            .Returns(Task.FromResult(false))
            .Verifiable();
        PaymentsRepository
            .Setup(repository => repository.Add(It.IsAny<PostPaymentRequest>(), PaymentStatus.Declined))
            .Returns(Task.FromResult(PaymentGatewayTestFixtures.PaymentRequest.ToEntity(PaymentStatus.Declined)))
            .Verifiable();
        var client = TestHttpClientFactory();

        // Act
        var response = await client.PostAsync($"/api/payments/create-payment", JsonContent.Create(PaymentGatewayTestFixtures.PaymentRequest));
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        BankService.Verify(bank => bank.MakePaymentAsync(It.IsAny<BankPaymentRequest>()), Times.Exactly(1));
        PaymentsRepository.Verify(repository => repository.Add(It.IsAny<PostPaymentRequest>(), PaymentStatus.Declined), Times.Exactly(1));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(PaymentStatus.Declined, paymentResponse!.Status);
    }

    [Fact]
    public async Task Returns400IfPaymentRequestRejected()
    {
        // Act
        var response = await DefaultClient.PostAsync($"/api/payments/create-payment", JsonContent.Create(PaymentGatewayTestFixtures.RejectedPaymentRequest));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}