using GameHub.API.Dtos.Purchases;
using GameHub.API.Validation.Abstractions;
using GameHub.API.Validation.Models;

namespace GameHub.API.Validation.Purchases;

public sealed class PurchaseHistoryQueryValidator
    : IValidator<PurchaseHistoryQuery>
{
    public ValidationErrors Validate(PurchaseHistoryQuery query)
    {
        var validation = new ValidationErrors();

        if (query.Page < 1)
        {
            validation.Add(
                "Page must be greater than zero.");
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            validation.Add(
                "Page size must be between 1 and 100.");
        }

        return validation;
    }
}