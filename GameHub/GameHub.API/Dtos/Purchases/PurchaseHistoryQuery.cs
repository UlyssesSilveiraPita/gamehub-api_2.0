using GameHub.API.Enums;

namespace GameHub.API.Dtos.Purchases;

public class PurchaseHistoryQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public PurchaseStatus? Status { get; set; }

    public string SortBy { get; set; } = "CreatedAt";

    public string SortDirection { get; set; } = "desc";
}
