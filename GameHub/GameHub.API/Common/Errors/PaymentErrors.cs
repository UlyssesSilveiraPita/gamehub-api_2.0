using GameHub.API.Common.Results;

namespace GameHub.API.Common.Errors;

public static class PaymentErrors
{
    public static readonly Error PurchaseNotFound =
        new("Payment.PurchaseNotFound", "Purchase not found.");

    public static readonly Error PaymentNotFound =
        new("Payment.PaymentNotFound", "Payment not found.");

    public static readonly Error PurchaseNotPending =
        new("Payment.PurchaseNotPending", "The purchase is no longer pending.");

    public static readonly Error PaymentNotPending =
        new("Payment.PaymentNotPending", "The payment is no longer pending.");

    public static readonly Error ActivePaymentAlreadyExists =
        new("Payment.ActivePaymentAlreadyExists","This purchase already has an active payment.");
}
