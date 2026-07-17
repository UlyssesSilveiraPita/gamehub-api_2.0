namespace GameHub.API.Dtos.Purchases;

public class CreatePurchaseRequest
{
    public int GameProductId { get; set; } 
    public int Quantity { get; set; }
}
