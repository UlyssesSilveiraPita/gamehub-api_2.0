using GameHub.API.Data;
using GameHub.API.Entities;
using GameHub.API.Enums;
using GameHub.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GameHub.API.Services.Commerce;

public class PaymentService : IPaymentService
{
    private readonly GameHubDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(GameHubDbContext context,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Payment> CreatePaymentAsync(
        int purchaseId,
        string userId,
        PaymentMethod paymentMethod)
    {

        _logger.LogInformation(
            "Starting payment creation. PurchaseId: {PurchaseId} | UserId: {UserId} | PaymentMethod: {PaymentMethod}",
            purchaseId,
            userId,
            paymentMethod);

        var purchase = await _context.Purchases
            .Include(p => p.Payments)
            .FirstOrDefaultAsync(p =>
                p.Id == purchaseId &&
                p.UserId == userId);

        if (purchase is null) 
        { 
            _logger.LogWarning(
                "Payment creation rejected because purchase was not found. PurchaseId: {PurchaseId} | UserId: {UserId}",
                purchaseId,
                userId);
        
            throw new KeyNotFoundException("Purchase not found.");
        }


        if (purchase.Status != PurchaseStatus.Pending)
        {
            _logger.LogWarning(
               "Payment creation rejected because purchase is not pending. PurchaseId: {PurchaseId} | Status: {PurchaseStatus}",
               purchaseId,
               purchase.Status);

            throw new InvalidOperationException(
                "Only pending purchases can receive a payment.");
        }


        if (purchase.Payments.Any(p =>
            p.Status == PaymentStatus.Pending ||
            p.Status == PaymentStatus.Paid))
        {
            _logger.LogWarning(
                "Payment creation rejected because an active payment already exists. PurchaseId: {PurchaseId}",
                purchaseId);

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

        _logger.LogInformation(
             "Payment created successfully. PaymentId: {PaymentId} | PurchaseId: {PurchaseId} | Amount: {Amount} | Currency: {Currency}",
            payment.Id,
            purchase.Id,
            payment.Amount,
            payment.Currency);

        return payment;
    }

    public async Task<Payment> ApprovePaymentAsync(
        int paymentId,
        string userId)
    {

        _logger.LogInformation(
            "Approving payment. PaymentId: {PaymentId} | UserId: {UserId}",
            paymentId,
            userId);

        var payment = await _context.Payments
            .Include(p => p.Purchase)
            .FirstOrDefaultAsync(p =>
                p.Id == paymentId &&
                p.Purchase.UserId == userId);

        if (payment is null) 
        {
            _logger.LogWarning(
                "Payment approval rejected because payment was not found. PaymentId: {PaymentId} | UserId: {UserId}",
                paymentId,
                userId);

            throw new KeyNotFoundException("Payment not found.");
        }

        var externalTransactionId = Guid.NewGuid().ToString();

        payment.MarkAsPaid(externalTransactionId);
        payment.Purchase.MarkAsPaid();
        
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Payment approved successfully. PaymentId: {PaymentId} | PurchaseId: {PurchaseId} | TransactionId: {TransactionId}",
            payment.Id,
            payment.PurchaseId,
            payment.ExternalTransactionId);

        return payment;
    }

    public async Task<Payment> FailPaymentAsync(
        int paymentId,
        string userId)
    {
        _logger.LogInformation(
            "Failing payment. PaymentId: {PaymentId} | UserId: {UserId}",
            paymentId,
            userId);

        var payment = await _context.Payments
            .Include(p => p.Purchase)
            .FirstOrDefaultAsync(p =>
                p.Id == paymentId &&
                p.Purchase.UserId == userId);

        if (payment is null)
        {
            _logger.LogWarning(
                "Payment failure request rejected because payment was not found. PaymentId: {PaymentId} | UserId: {UserId}",
                paymentId,
                userId);

            throw new KeyNotFoundException("Payment not found.");
        }

       
        payment.MarkAsFailed();

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Payment marked as failed. PaymentId: {PaymentId} | PurchaseId: {PurchaseId}",
            payment.Id,
            payment.PurchaseId);

        return payment;
    }


}
