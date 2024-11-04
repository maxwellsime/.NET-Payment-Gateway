using Microsoft.Extensions.Configuration;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Services;

using Testcontainers.MongoDb;

namespace PaymentGateway.Api.Tests.Services;

public class PaymentRepositoryTests
{
    private readonly PaymentsRepository _repository;
    private readonly MongoDbContainer _testContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .WithPortBinding(80, 8080)
        .Build();

    public PaymentRepositoryTests() {
        var inMemoryConfig = new Dictionary<string, string?> {
            { "ConnectionStrings:DbConnection", PaymentGatewayTestFixtures.MongoDB }
        };
        var testConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        _repository = new PaymentsRepository(testConfig);
        _testContainer.StartAsync();
    }

    [Fact]
    public async Task GetPriorPaymentSuccessfully()
    {
        // Arrange
        var expectedResponse = _repository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Authorized).Result;

        // Act
        var response = await _repository.GetById(expectedResponse.Id!);

        // Assert
        Assert.Equal(expectedResponse.Id, response.Id);
    }

    [Fact]
    public async Task GetThrowsExceptionUponInvalidGuid()
    {
        var response = await _repository.GetById("");

        // Assert
        Assert.Null(response);
    }
}