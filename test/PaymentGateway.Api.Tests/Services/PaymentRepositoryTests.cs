using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

public class PaymentRepositoryTests
{
    private static readonly PaymentsRepository Repository = new PaymentsRepository();

    [Fact]
    public async Task GetPriorPaymentSuccessfully() 
    {
        // Arrange
        var expectedResponse = Repository.Add(PaymentGatewayTestFixtures.PaymentRequest, PaymentStatus.Authorized);

        // Act
        var response =  Repository.Get(expectedResponse.Id!.Value);

        // Assert
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async Task GetThrowsExceptionUponInvalidGuid()
    {
        // Assert 
        Assert.Throws<ArgumentNullException>(() => Repository.Get(Guid.Empty));
    }
}