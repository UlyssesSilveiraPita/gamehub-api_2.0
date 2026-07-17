using GameHub.API.Enums;

namespace GameHub.API.Dtos.Payments;

public class CreatePaymentRequest
{
    public PaymentMethod PaymentMethod { get; set; }
}
