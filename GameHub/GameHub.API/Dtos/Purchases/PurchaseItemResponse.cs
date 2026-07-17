namespace GameHub.API.Dtos.Purchases;

public class PurchaseItemResponse
{
    public int GameProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}
