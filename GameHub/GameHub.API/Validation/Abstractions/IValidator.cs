using GameHub.API.Validation.Models;

namespace GameHub.API.Validation.Abstractions;

public interface IValidator<in T>
{
    ValidationErrors Validate(T instance);
}
