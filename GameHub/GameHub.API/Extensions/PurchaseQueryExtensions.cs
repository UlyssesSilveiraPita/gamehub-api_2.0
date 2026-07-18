using GameHub.API.Entities;
using GameHub.API.Enums;

namespace GameHub.API.Extensions;

public static class PurchaseQueryExtensions
{
    public static IQueryable<Purchase> FilterByStatus(
        this IQueryable<Purchase> query,
        PurchaseStatus? status)
    {
        if (!status.HasValue) 
            return query; 
        
              
        return query.Where(p => p.Status == status.Value);
    }

    public static IQueryable<Purchase> ApplySorting(
    this IQueryable<Purchase> query,
    string sortBy,
    string sortDirection)
    {
        var ascending = sortDirection.Equals(
            "asc",
            StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "totalamount" => ascending
                ? query.OrderBy(p => p.TotalAmount)
                : query.OrderByDescending(p => p.TotalAmount),

            _ => ascending
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt)
        };
    }

    public static IQueryable<Purchase> ApplyPagination(
        this IQueryable<Purchase> query,
        int page,
        int pageSize)
    {
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}
