using GameHub.API.Data;
using GameHub.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameHub.API.Services;

public class PurchaseService
{
    private readonly GameHubDbContext _context;

    public PurchaseService(GameHubDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase> CreatePurchaseAsync(
        string userId, // usuario que esta comprando
        int gameProductId, // produto escolhido
        int quantity) // quantidade desejada
    {
        var product = await _context.GameProducts
            .FirstOrDefaultAsync(product =>
                product.Id == gameProductId &&
                product.IsActive);

        if (product is null)
        {
            throw new InvalidOperationException("Game product not found or inactive.");
        }

        if(quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        var item = new PurchaseItem
        {
            GameProductId = gameProductId,
            ProductName = product.Name, //mantem o nome do produto na hora da compra
            UnitPrice = product.Price, // mantem o valor do produto na hora da compra
            Quantity = quantity
        };

        item.Validate();

        var purchase = new Purchase
        {
            UserId = userId,
            Currency = product.Currency
        };

        purchase.AddItem(item);

        _context.Purchases.Add(purchase);

        await _context.SaveChangesAsync();
        
        return purchase;

    }




}
