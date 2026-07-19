using GameHub.API.Common.Results;
using GameHub.API.Dtos.Common;
using GameHub.API.Validation.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameHub.API.Extensions;

public static class ControllerExtensions
{
    public static BadRequestObjectResult ValidationFailed(
        this ControllerBase controller,
        ValidationErrors validation)
    {
        return controller.BadRequest(
            new ValidationErrorResponse
            {
                Errors = validation.Errors
            });
    }

    public static BadRequestObjectResult BadRequestError(
        this ControllerBase controller,
        Error error)
    {
        return controller.BadRequest(
            new ApiErrorResponse
            {
                Code = error.Code,
                Message = error.Message
            });
    }

    public static NotFoundObjectResult NotFoundError(
        this ControllerBase controller,
        Error error)
    {
        return controller.NotFound(
            new ApiErrorResponse
            {
                Code = error.Code,
                Message = error.Message
            });
    }
}
