using GameHub.API.Common.Results;

namespace GameHub.API.Common.Errors;

public static class PurchaseErrors
{
    public static readonly Error ProductNotFound =
        new(
            "Purchase.ProductNotFound",
            "Game product not found or inactive.");

    public static readonly Error InvalidQuantity =
        new(
            "Purchase.InvalidQuantity",
            "Quantity must be greater than zero.");
}
