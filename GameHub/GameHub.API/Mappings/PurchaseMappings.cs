using GameHub.API.Dtos.Purchases;
using GameHub.API.Entities;

namespace GameHub.API.Mappings;

public static class PurchaseMappings
{
    public static PurchaseResponse ToResponse(this Purchase purchase)
    {
        return new PurchaseResponse
        {
            Id = purchase.Id,
            Status = purchase.Status.ToString(),
            TotalAmount = purchase.TotalAmount,
            Currency = purchase.Currency,
            CreatedAt = purchase.CreatedAt,
            Items = purchase.Items
                .Select(item => new PurchaseItemResponse
                {
                    GameProductId = item.GameProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                })
                .ToList()
        };
    }
}