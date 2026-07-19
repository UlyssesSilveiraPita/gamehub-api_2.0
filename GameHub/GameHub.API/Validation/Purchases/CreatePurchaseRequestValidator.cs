using GameHub.API.Dtos.Purchases;
using GameHub.API.Validation.Abstractions;
using GameHub.API.Validation.Models;

namespace GameHub.API.Validation.Purchases;

public sealed class CreatePurchaseRequestValidator 
    : IValidator<CreatePurchaseRequest>
{
    public ValidationErrors Validate(CreatePurchaseRequest request)
    {
        var validation = new ValidationErrors();

        if (request.GameProductId <= 0)
        {
            validation.Add(
                "GameProductId must be greater than zero.");
        }

        if (request.Quantity <= 0)
        {
            validation.Add(
                "Quantity must be greater than zero.");
        }

        return validation;
    }
}
