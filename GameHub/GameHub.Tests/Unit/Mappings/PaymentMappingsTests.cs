using FluentAssertions;
using GameHub.API.Entities;
using GameHub.API.Enums;
using GameHub.API.Mappings;

namespace GameHub.Tests.Unit.Mappings;

public class PaymentMappingsTests
{
    [Fact]
    public void ToResponse_ShouldMapAllPaymentProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        var payment = new Payment
        {
            Id = 10,
            PurchaseId = 5,
            Status = PaymentStatus.Paid,
            PaymentMethod = PaymentMethod.CreditCard,
            Amount = 119.80m,
            Currency = "BRL",
            IdempotencyKey = "payment-key-123",
            ExternalTransactionId = "transaction-456",
            CreatedAt = createdAt
        };

        // Act
        var response = payment.ToResponse();

        // Assert
        response.Id.Should().Be(payment.Id);
        response.PurchaseId.Should().Be(payment.PurchaseId);
        response.Status.Should().Be(payment.Status.ToString());
        response.PaymentMethod.Should().Be(payment.PaymentMethod.ToString());
        response.Amount.Should().Be(payment.Amount);
        response.Currency.Should().Be(payment.Currency);
        response.IdempotencyKey.Should().Be(payment.IdempotencyKey);
        response.ExternalTransactionId.Should().Be(payment.ExternalTransactionId);
        response.CreatedAt.Should().Be(createdAt);
    }
}