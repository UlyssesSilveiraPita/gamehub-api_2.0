using GameHub.API.Data;
using GameHub.API.Entities;
using GameHub.API.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GameHub.API.Services;

public class PaymentService
{
    private readonly GameHubDbContext _context;

    public PaymentService(GameHubDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> CreatePaymentAsync(
    int purchaseId,
    string userId,
    PaymentMethod paymentMethod)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Payments)
            .FirstOrDefaultAsync(p =>
                p.Id == purchaseId &&
                p.UserId == userId);

        if (purchase is null)
            throw new KeyNotFoundException("Purchase not found.");

        if (purchase.Status != PurchaseStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending purchases can receive a payment.");
        }

        if (purchase.Payments.Any(p =>
            p.Status == PaymentStatus.Pending ||
            p.Status == PaymentStatus.Paid))
        {
            throw new InvalidOperationException(
                "This purchase already has an active payment.");
        }

        var payment = new Payment
        {
            PurchaseId = purchase.Id,
            Amount = purchase.TotalAmount,
            Currency = purchase.Currency,
            PaymentMethod = paymentMethod,
            Status = PaymentStatus.Pending,
            IdempotencyKey = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };

        payment.Validate();

        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment> ApprovePaymentAsync(
    int paymentId,
    string userId)
    {
        var payment = await _context.Payments
            .Include(p => p.Purchase)
            .FirstOrDefaultAsync(p =>
                p.Id == paymentId &&
                p.Purchase.UserId == userId);

        if (payment is null)
            throw new KeyNotFoundException("Payment not found.");

        var externalTransactionId = Guid.NewGuid().ToString();

        payment.MarkAsPaid(externalTransactionId);
        payment.Purchase.MarkAsPaid();
        
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment> FailPaymentAsync(
    int paymentId,
    string userId)
    {
        var payment = await _context.Payments
            .Include(p => p.Purchase)
            .FirstOrDefaultAsync(p =>
                p.Id == paymentId &&
                p.Purchase.UserId == userId);

        if (payment is null)
            throw new KeyNotFoundException("Payment not found.");

        payment.MarkAsFailed();

        await _context.SaveChangesAsync();

        return payment;
    }


}
