using FluentAssertions;
using GameHub.API.Common.Errors;
using GameHub.API.Entities;
using GameHub.API.Services.Commerce;
using GameHub.Tests.Builders;
using GameHub.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace GameHub.Tests.Unit.Services;

public class PurchaseServiceTests
{
    [Fact]
    public async Task CreatePurchaseAsync_ShouldReturnInvalidQuantity_WhenQuantityIsZero()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var service = new PurchaseService(context);

        // Act
        var result = await service.CreatePurchaseAsync(
            userId: "user-1",
            gameProductId: 1,
            quantity: 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PurchaseErrors.InvalidQuantity);

        context.Purchases.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePurchaseAsync_ShouldReturnInvalidQuantity_WhenQuantityIsNegative()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var service = new PurchaseService(context);

        // Act
        var result = await service.CreatePurchaseAsync(
            userId: "user-1",
            gameProductId: 1,
            quantity: -1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PurchaseErrors.InvalidQuantity);

        context.Purchases.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePurchaseAsync_ShouldReturnProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var service = new PurchaseService(context);

        // Act
        var result = await service.CreatePurchaseAsync(
            userId: "user-1",
            gameProductId: 999,
            quantity: 1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PurchaseErrors.ProductNotFound);

        context.Purchases.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePurchaseAsync_ShouldReturnProductNotFound_WhenProductIsInactive()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        context.GameProducts.Add(
            new GameProductBuilder()
                .Inactive()
                .Build());

        await context.SaveChangesAsync();

        var service = new PurchaseService(context);

        // Act
        var result = await service.CreatePurchaseAsync(
            "user-1",
            1,
            1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PurchaseErrors.ProductNotFound);

        context.Purchases.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePurchaseAsync_ShouldCreatePurchase_WhenProductIsValid()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        context.GameProducts.Add(new GameProduct
        {
            Id = 1,
            Name = "Wild Hunter",
            Price = 59.90m,
            Currency = "BRL",
            IsActive = true
        });

        await context.SaveChangesAsync();

        var service = new PurchaseService(context);

        // Act
        var result = await service.CreatePurchaseAsync(
            "user-1",
            1,
            2);

        // Assert

        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

        result.Value.UserId.Should().Be("user-1");

        result.Value.Currency.Should().Be("BRL");

        result.Value.TotalAmount.Should().Be(119.80m);

        result.Value.Items.Should().ContainSingle();

        var item = result.Value.Items.Single();

        item.ProductName.Should().Be("Wild Hunter");

        item.UnitPrice.Should().Be(59.90m);

        item.Quantity.Should().Be(2);

        item.TotalPrice.Should().Be(119.80m);

        context.Purchases.Should().ContainSingle();
    }

    [Fact]
    public async Task GetPurchaseByIdAsync_ShouldReturnPurchase_WhenPurchaseBelongsToUser()
    {
        // Arrange
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext =
            TestDbContextFactory.Create(databaseName))
        {
            var purchase = new Purchase
            {
                Id = 1,
                UserId = "user-1",
                Currency = "BRL"
            };

            purchase.AddItem(new PurchaseItem
            {
                GameProductId = 1,
                ProductName = "Wild Hunter",
                UnitPrice = 59.90m,
                Quantity = 2
            });

            seedContext.Purchases.Add(purchase);

            await seedContext.SaveChangesAsync();
        }

        await using var context =
            TestDbContextFactory.Create(databaseName);

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchaseByIdAsync(
            purchaseId: 1,
            userId: "user-1");

        // Assert
        result.Should().NotBeNull();

        result!.Id.Should().Be(1);
        result.UserId.Should().Be("user-1");
        result.TotalAmount.Should().Be(119.80m);

        result.Items.Should().ContainSingle();

        var item = result.Items.Single();

        item.ProductName.Should().Be("Wild Hunter");
        item.Quantity.Should().Be(2);
    }

    [Fact] 
    public async Task GetPurchaseByIdAsync_ShouldReturnNull_WhenPurchaseBelongsToAnotherUser()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        context.Purchases.Add(new Purchase
        {
            Id = 1,
            UserId = "user-owner",
            Currency = "BRL"
        });

        await context.SaveChangesAsync();

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchaseByIdAsync(
            purchaseId: 1,
            userId: "user-other");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPurchaseByIdAsync_ShouldReturnNull_WhenPurchaseDoesNotExist()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchaseByIdAsync(
            purchaseId: 999,
            userId: "user-1");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPurchasesByUserAsync_ShouldReturnOnlyPurchasesFromRequestedUser()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        context.Purchases.AddRange(
            new Purchase
            {
                Id = 1,
                UserId = "user-1",
                Currency = "BRL"
            },
            new Purchase
            {
                Id = 2,
                UserId = "user-1",
                Currency = "BRL"
            },
            new Purchase
            {
                Id = 3,
                UserId = "user-2",
                Currency = "BRL"
            });

        await context.SaveChangesAsync();

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchasesByUserAsync("user-1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(
            purchase => purchase.UserId == "user-1");

        result.Should().NotContain(
            purchase => purchase.UserId == "user-2");
    }

    [Fact]
    public async Task GetPurchasesByUserAsync_ShouldReturnEmptyList_WhenUserHasNoPurchases()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        context.Purchases.Add(new Purchase
        {
            Id = 1,
            UserId = "another-user",
            Currency = "BRL"
        });

        await context.SaveChangesAsync();

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchasesByUserAsync("user-1");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPurchasesByUserAsync_ShouldIncludePurchaseItems()
    {
        // Arrange
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext =
            TestDbContextFactory.Create(databaseName))
        {
            var expectedPurchase = new Purchase
            {
                Id = 1,
                UserId = "user-1",
                Currency = "BRL"
            };

            expectedPurchase.AddItem(new PurchaseItem
            {
                GameProductId = 1,
                ProductName = "Wild Hunter",
                UnitPrice = 59.90m,
                Quantity = 2
            });

            seedContext.Purchases.Add(expectedPurchase);

            await seedContext.SaveChangesAsync();
        }

        await using var context =
            TestDbContextFactory.Create(databaseName);

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchasesByUserAsync("user-1");

        // Assert
        result.Should().ContainSingle();

        var returnedPurchase = result.Single();
        var item = returnedPurchase.Items.Single();

        returnedPurchase.Items.Should().ContainSingle();

        item.ProductName.Should().Be("Wild Hunter");
        item.Quantity.Should().Be(2);

        returnedPurchase.TotalAmount.Should().Be(119.80m);
    }

    [Fact]
    public async Task GetPurchasesByUserAsync_ShouldOrderPurchasesByCreatedAtDescending()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var oldestPurchase = new Purchase
        {
            Id = 1,
            UserId = "user-1",
            Currency = "BRL",
            CreatedAt = new DateTime(
                2026, 1, 10, 10, 0, 0,
                DateTimeKind.Utc)
        };

        var newestPurchase = new Purchase
        {
            Id = 2,
            UserId = "user-1",
            Currency = "BRL",
            CreatedAt = new DateTime(
                2026, 7, 20, 10, 0, 0,
                DateTimeKind.Utc)
        };

        context.Purchases.AddRange(
            oldestPurchase,
            newestPurchase);

        await context.SaveChangesAsync();

        var service = new PurchaseService(context);

        // Act
        var result = await service.GetPurchasesByUserAsync("user-1");

        // Assert
        result.Should().HaveCount(2);

        result.Select(purchase => purchase.Id)
            .Should()
            .ContainInOrder(2, 1);
    }
}
