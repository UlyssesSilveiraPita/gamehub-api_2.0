using GameHub.API.Enums;

namespace GameHub.API.Entities;

public class Purchase
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "BRL";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public ICollection<PurchaseItem> Items { get; set; } = [];

    public ICollection<Payment> Payments { get; set; } = [];


    //=======================\\
    // Controle de Transacoes\\
    //=======================\\

    public void MarkAsPaid()
    {
        if (Status != PurchaseStatus.Pending)
        {
            throw new InvalidOperationException("Only pending purchases can be marked as paid.");
        }

        Status = PurchaseStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != PurchaseStatus.Pending)
        {
            throw new InvalidOperationException("Only pending purchases can be cancelled.");
        }

        Status = PurchaseStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != PurchaseStatus.Paid)
        {
            throw new InvalidOperationException("Only paid purchases can be refunded.");
        }

        Status = PurchaseStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CalculateTotal()
    {
        TotalAmount = Items.Sum(item => item.TotalPrice);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItem(PurchaseItem item)
    {
        Items.Add(item);
        CalculateTotal();
    }



}
