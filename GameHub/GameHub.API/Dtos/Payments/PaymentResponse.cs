namespace GameHub.API.Dtos.Payments;

public class PaymentResponse
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? ExternalTransactionId { get; set; }
}
