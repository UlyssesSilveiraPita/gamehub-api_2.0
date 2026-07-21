using GameHub.API.Dtos.Payments;
using GameHub.API.Entities;

namespace GameHub.API.Mappings;

public static class PaymentMappings
{
    public static PaymentResponse ToResponse(this Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            PurchaseId = payment.PurchaseId,
            Status = payment.Status.ToString(),
            PaymentMethod = payment.PaymentMethod.ToString(),
            Amount = payment.Amount,
            Currency = payment.Currency,
            IdempotencyKey = payment.IdempotencyKey,
            ExternalTransactionId = payment.ExternalTransactionId,
            CreatedAt = payment.CreatedAt
        };
    }
}