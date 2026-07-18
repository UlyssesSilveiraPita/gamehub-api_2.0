using GameHub.API.Data;
using GameHub.API.Dtos.Common;
using GameHub.API.Dtos.Purchases;
using GameHub.API.Entities;
using GameHub.API.Enums;
using GameHub.API.Extensions;
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

    //pedido de compra do usuario\\
    public async Task<Purchase?> GetPurchaseByIdAsync(
        int purchaseId,
        string userId)
    {
        return await _context.Purchases
            .Include(p => p.Items) // trazer os itens junto da compra
            .FirstOrDefaultAsync(p => 
                p.Id == purchaseId && 
                p.UserId == userId);
    }

    //lista todas as compras do usuario\\
    public async Task<List<Purchase>> GetPurchasesByUserAsync(
    string userId)
    {
        return await _context.Purchases
            .AsNoTracking() //EF Core não precisa acompanhar as entidades.
            .Include(p => p.Items)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt) // lista as compras do mais recente para o mais antigo
            .ToListAsync();
    }

    public async Task<PagedResponse<PurchaseHistoryResponse>> GetPurchaseHistoryAsync(
        string userId,
        PurchaseHistoryQuery request)
    {
        var query = _context.Purchases
            .AsNoTracking()
            .Where(p => p.UserId == userId);

        query = query.FilterByStatus(request.Status);

        var totalItems = await query.CountAsync();

        query = query.ApplySorting(request.SortBy, request.SortDirection);

        var items = await query
            .ApplyPagination(request.Page, request.PageSize)
            .Select(p => new PurchaseHistoryResponse
            {
                Id = p.Id,
                Status = p.Status.ToString(),
                TotalAmount = p.TotalAmount,
                Currency = p.Currency,
                CreatedAt = p.CreatedAt,
                TotalItems = p.Items.Sum(item => item.Quantity)
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)request.PageSize);

        return new PagedResponse<PurchaseHistoryResponse>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = items
        };
    }

}
