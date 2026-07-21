using FluentAssertions;
using GameHub.API.Entities;
using GameHub.API.Enums;
using GameHub.API.Services.Commerce;
using GameHub.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameHub.Tests.Unit.Services;

public class PaymentServiceTests
{
    private readonly ILogger<PaymentService> _logger =
        NullLogger<PaymentService>.Instance;

    [Fact]
    public async Task CreatePaymentAsync_ShouldCreatePayment_WhenPurchaseIsValid()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var purchase = new Purchase
        {
            UserId = "user-1",
            Status = PurchaseStatus.Pending,
            TotalAmount = 119.80m,
            Currency = "BRL"
        };

        context.Purchases.Add(purchase);
        await context.SaveChangesAsync();

        var service = new PaymentService(context, _logger);

        // Act
        var payment = await service.CreatePaymentAsync(
            purchase.Id,
            "user-1",
            PaymentMethod.Pix);

        // Assert
        payment.Should().NotBeNull();

        payment.PurchaseId.Should().Be(purchase.Id);
        payment.Amount.Should().Be(119.80m);
        payment.Currency.Should().Be("BRL");
        payment.PaymentMethod.Should().Be(PaymentMethod.Pix);
        payment.Status.Should().Be(PaymentStatus.Pending);

        payment.IdempotencyKey.Should().NotBeNullOrWhiteSpace();
        payment.CreatedAt.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(5));

        context.Payments.Should().ContainSingle();
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldThrowKeyNotFoundException_WhenPurchaseDoesNotExist()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var service = new PaymentService(context, _logger);

        // Act
        var action = async () =>
            await service.CreatePaymentAsync(
                purchaseId: 999,
                userId: "user-1",
                paymentMethod: PaymentMethod.Pix);

        // Assert
        await action.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Purchase not found.");

        context.Payments.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldThrowKeyNotFoundException_WhenPurchaseBelongsToAnotherUser()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var purchase = new Purchase
        {
            UserId = "purchase-owner",
            Status = PurchaseStatus.Pending,
            TotalAmount = 59.90m,
            Currency = "BRL"
        };

        context.Purchases.Add(purchase);
        await context.SaveChangesAsync();

        var service = new PaymentService(context, _logger);

        // Act
        var action = async () =>
            await service.CreatePaymentAsync(
                purchase.Id,
                "different-user",
                PaymentMethod.CreditCard);

        // Assert
        await action.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Purchase not found.");

        context.Payments.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldThrowInvalidOperationException_WhenPurchaseIsNotPending()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var purchase = new Purchase
        {
            UserId = "user-1",
            Status = PurchaseStatus.Paid,
            TotalAmount = 119.80m,
            Currency = "BRL"
        };

        context.Purchases.Add(purchase);
        await context.SaveChangesAsync();

        var service = new PaymentService(context, _logger);

        // Act
        var action = async () =>
            await service.CreatePaymentAsync(
                purchase.Id,
                "user-1",
                PaymentMethod.Pix);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(
                "Only pending purchases can receive a payment.");

        context.Payments.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldThrowInvalidOperationException_WhenActivePaymentAlreadyExists()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var purchase = new Purchase
        {
            UserId = "user-1",
            Status = PurchaseStatus.Pending,
            TotalAmount = 59.90m,
            Currency = "BRL"
        };

        context.Purchases.Add(purchase);
        await context.SaveChangesAsync();

        context.Payments.Add(new Payment
        {
            PurchaseId = purchase.Id,
            Amount = purchase.TotalAmount,
            Currency = purchase.Currency,
            PaymentMethod = PaymentMethod.Pix,
            Status = PaymentStatus.Pending,
            IdempotencyKey = Guid.NewGuid().ToString()
        });

        await context.SaveChangesAsync();

        var service = new PaymentService(context, _logger);

        // Act
        var action = async () =>
            await service.CreatePaymentAsync(
                purchase.Id,
                "user-1",
                PaymentMethod.CreditCard);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("This purchase already has an active payment.");

        context.Payments.Should().HaveCount(1);
    }
}