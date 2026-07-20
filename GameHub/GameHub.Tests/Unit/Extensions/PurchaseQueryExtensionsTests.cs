using FluentAssertions;
using GameHub.API.Dtos.Purchases;
using GameHub.API.Entities;
using GameHub.API.Enums;
using GameHub.API.Extensions;
using GameHub.API.Services.Commerce;
using GameHub.Tests.Helpers;

namespace GameHub.Tests.Unit.Extensions;

public class PurchaseQueryExtensionsTests
{
    //FilterByStatus\\

    [Fact]
    public void FilterByStatus_ShouldReturnOnlyMatchingPurchases_WhenStatusIsProvided()
    {
        // Arrange
        var purchases = new List<Purchase>
    {
        new()
        {
            Id = 1,
            UserId = "user-1",
            Currency = "BRL",
            Status = PurchaseStatus.Pending
        },
        new()
        {
            Id = 2,
            UserId = "user-1",
            Currency = "BRL",
            Status = PurchaseStatus.Paid
        },
        new()
        {
            Id = 3,
            UserId = "user-1",
            Currency = "BRL",
            Status = PurchaseStatus.Paid
        }
    }.AsQueryable();

        // Act
        var result = purchases
            .FilterByStatus(PurchaseStatus.Paid)
            .ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(
            purchase => purchase.Status == PurchaseStatus.Paid);
    }

    [Fact]
    public void FilterByStatus_ShouldReturnOriginalQuery_WhenStatusIsNull()
    {
        // Arrange
        var purchases = new List<Purchase>
    {
        new()
        {
            Id = 1,
            UserId = "user-1",
            Currency = "BRL",
            Status = PurchaseStatus.Pending
        },
        new()
        {
            Id = 2,
            UserId = "user-1",
            Currency = "BRL",
            Status = PurchaseStatus.Paid
        }
    }.AsQueryable();

        // Act
        var result = purchases
            .FilterByStatus(null)
            .ToList();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void FilterByStatus_ShouldReturnEmpty_WhenNoPurchaseMatchesStatus()
    {
        // Arrange
        var purchases = new List<Purchase>
    {
        new()
        {
            Id = 1,
            UserId = "user-1",
            Currency = "BRL",
            Status = PurchaseStatus.Pending
        }
    }.AsQueryable();

        // Act
        var result = purchases
            .FilterByStatus(PurchaseStatus.Paid)
            .ToList();

        // Assert
        result.Should().BeEmpty();
    }

    //ApplySorting por TotalAmount\\

    [Fact]
    public void ApplySorting_ShouldSortByTotalAmountAscending()
    {
        // Arrange
        var purchases = new List<Purchase>
    {
        CreatePurchase(id: 1, totalAmount: 150m),
        CreatePurchase(id: 2, totalAmount: 50m),
        CreatePurchase(id: 3, totalAmount: 100m)
    }.AsQueryable();

        // Act
        var result = purchases
            .ApplySorting("totalAmount", "asc")
            .ToList();

        // Assert
        result.Select(purchase => purchase.TotalAmount)
            .Should()
            .ContainInOrder(50m, 100m, 150m);
    }

    [Fact]
    public void ApplySorting_ShouldSortByTotalAmountDescending()
    {
        // Arrange
        var purchases = new List<Purchase>
    {
        CreatePurchase(id: 1, totalAmount: 150m),
        CreatePurchase(id: 2, totalAmount: 50m),
        CreatePurchase(id: 3, totalAmount: 100m)
    }.AsQueryable();

        // Act
        var result = purchases
            .ApplySorting("totalAmount", "desc")
            .ToList();

        // Assert
        result.Select(purchase => purchase.TotalAmount)
            .Should()
            .ContainInOrder(150m, 100m, 50m);
    }

    private static Purchase CreatePurchase(
        int id,
        decimal totalAmount,
        DateTime? createdAt = null)
    {
        var purchase = new Purchase
        {
            Id = id,
            UserId = "user-1",
            Currency = "BRL",
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

        purchase.AddItem(new PurchaseItem
        {
            GameProductId = id,
            ProductName = $"Product {id}",
            UnitPrice = totalAmount,
            Quantity = 1
        });

        return purchase;
    }

    //ApplySorting por CreatedAt\\

    [Fact]
    public void ApplySorting_ShouldSortByCreatedAtAscending()
    {
        // Arrange
        var oldestDate = new DateTime(2026, 1, 1);
        var middleDate = new DateTime(2026, 4, 1);
        var newestDate = new DateTime(2026, 7, 1);

        var purchases = new List<Purchase>
    {
        CreatePurchase(1, 50m, newestDate),
        CreatePurchase(2, 50m, oldestDate),
        CreatePurchase(3, 50m, middleDate)
    }.AsQueryable();

        // Act
        var result = purchases
            .ApplySorting("CreatedAt", "asc")
            .ToList();

        // Assert
        result.Select(purchase => purchase.Id)
            .Should()
            .ContainInOrder(2, 3, 1);
    }

    [Fact]
    public void ApplySorting_ShouldSortByCreatedAtDescending_WhenSortByIsUnknown()
    {
        // Arrange
        var oldestDate = new DateTime(2026, 1, 1);
        var newestDate = new DateTime(2026, 7, 1);

        var purchases = new List<Purchase>
    {
        CreatePurchase(1, 50m, oldestDate),
        CreatePurchase(2, 50m, newestDate)
    }.AsQueryable();

        // Act
        var result = purchases
            .ApplySorting("invalid-field", "desc")
            .ToList();

        // Assert
        result.Select(purchase => purchase.Id)
            .Should()
            .ContainInOrder(2, 1);
    }

    //ApplyPagination\\

    [Fact]
    public void ApplyPagination_ShouldReturnRequestedPage()
    {
        // Arrange
        var purchases = Enumerable.Range(1, 5)
            .Select(id => CreatePurchase(id, id * 10m))
            .AsQueryable();

        // Act
        var result = purchases
            .ApplyPagination(page: 2, pageSize: 2)
            .ToList();

        // Assert
        result.Select(purchase => purchase.Id)
            .Should()
            .ContainInOrder(3, 4);
    }

    [Fact]
    public void ApplyPagination_ShouldReturnRemainingItems_OnLastPage()
    {
        // Arrange
        var purchases = Enumerable.Range(1, 5)
            .Select(id => CreatePurchase(id, id * 10m))
            .AsQueryable();

        // Act
        var result = purchases
            .ApplyPagination(page: 3, pageSize: 2)
            .ToList();

        // Assert
        result.Should().ContainSingle();
        result.Single().Id.Should().Be(5);
    }


    [Fact]
    public void ApplyPagination_ShouldReturnEmpty_WhenPageExceedsAvailableItems()
    {
        // Arrange
        var purchases = Enumerable.Range(1, 3)
            .Select(id => CreatePurchase(id, id * 10m))
            .AsQueryable();

        // Act
        var result = purchases
            .ApplyPagination(page: 5, pageSize: 2)
            .ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPurchaseHistoryAsync_ShouldReturnOnlyPurchasesFromRequestedUser()
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

        var request = new PurchaseHistoryQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await service.GetPurchaseHistoryAsync(
            "user-1",
            request);

        // Assert
        result.TotalItems.Should().Be(2);
        result.Items.Should().HaveCount(2);

        result.Items
            .Select(item => item.Id)
            .Should()
            .BeEquivalentTo(new[] { 1, 2 });
    }
}
