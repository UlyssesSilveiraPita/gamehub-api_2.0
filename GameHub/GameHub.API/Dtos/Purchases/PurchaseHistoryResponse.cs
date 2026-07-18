namespace GameHub.API.Dtos.Purchases;

public class PurchaseHistoryResponse
{
    public int Id { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int TotalItems { get; set; }
}
