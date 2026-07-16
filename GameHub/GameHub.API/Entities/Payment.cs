using GameHub.API.Entities.Enums;

namespace GameHub.API.Entities;

public class Payment
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "BRL";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public PaymentMethod PaymentMethod { get; set; }

    public string IdempotencyKey { get; set; } = string.Empty;

    public string? ExternalTransactionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public Purchase Purchase { get; set; } = null!;

    public void MarkAsPaid(string externalTransactionId)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as paid.");
        }

        if (string.IsNullOrWhiteSpace(externalTransactionId))
        {
            throw new InvalidOperationException("External transaction id is required.");
        }

        Status = PaymentStatus.Paid;
        ExternalTransactionId = externalTransactionId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as failed.");
        }

        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Paid)
        {
            throw new InvalidOperationException("Only paid payments can be refunded.");
        }

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Validate()
    {
        if (Amount <= 0)
        {
            throw new InvalidOperationException("Payment amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(Currency))
        {
            throw new InvalidOperationException("Currency is required.");
        }

        if (string.IsNullOrWhiteSpace(IdempotencyKey))
        {
            throw new InvalidOperationException("Idempotency key is required.");
        }
    }
}
