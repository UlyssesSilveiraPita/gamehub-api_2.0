using GameHub.API.Entities;
using GameHub.API.Enums;

namespace GameHub.API.Services.Abstractions;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(
        int purchaseId,
        string userId,
        PaymentMethod paymentMethod);

    Task<Payment> ApprovePaymentAsync(
        int paymentId,
        string userId);

    Task<Payment> FailPaymentAsync(
        int paymentId,
        string userId);
}
