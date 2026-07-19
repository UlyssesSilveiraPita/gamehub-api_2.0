using GameHub.API.Dtos.Purchases;
using GameHub.API.Validation.Abstractions;
using GameHub.API.Validation.Purchases;
using Microsoft.Extensions.DependencyInjection;

namespace GameHub.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidation(
        this IServiceCollection services)
    {
        services.AddScoped<
            IValidator<CreatePurchaseRequest>,
            CreatePurchaseRequestValidator>();

        services.AddScoped<
            IValidator<PurchaseHistoryQuery>,
            PurchaseHistoryQueryValidator>();

        return services;
    }
}
