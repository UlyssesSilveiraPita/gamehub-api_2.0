using GameHub.API.Dtos.Common;
using GameHub.API.Dtos.Purchases;
using GameHub.API.Entities;

namespace GameHub.API.Services.Abstractions;

public interface IPurchaseService
{
    Task<Purchase> CreatePurchaseAsync(
        string userId,
        int gameProductId,
        int quantity);

    Task<Purchase?> GetPurchaseByIdAsync(
        int purchaseId,
        string userId);

    Task<List<Purchase>> GetPurchasesByUserAsync(
        string userId);

    Task<PagedResponse<PurchaseHistoryResponse>> GetPurchaseHistoryAsync(
        string userId,
        PurchaseHistoryQuery request);
}
