namespace GameHub.API.Entities;

public class PurchaseItem // momento da compra.
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public int GameProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity; // ja calcula o valor com a quantidade

    public Purchase Purchase { get; set; } = null!;

    public GameProduct GameProduct { get; set; } = null!;

    public void Validate()
    {
        if(string.IsNullOrWhiteSpace(ProductName))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        if(UnitPrice <= 0)
        {
            throw new InvalidOperationException("Unit price must be greater than zero.");
        }

        if(Quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }




    }

}
