using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaApp.Api.ExceptionHandling;
using PizzaApp.Api.Security;
using PizzaApp.Dtos.Common;
using PizzaApp.Services.Abstractions;

namespace PizzaApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                // When a controller action receives an invalid DTO, ASP.NET Core automatically returns a 400 Bad Request with a ProblemDetails body.
                // Invalid DTOs get the same ErrorResponse body as every other error (no ProblemDetails)
                options.SuppressMapClientErrors = true;
                options.InvalidModelStateResponseFactory = CreateValidationErrorResponse;
            });

        services.AddOpenApi();
        services.AddOpenApiDocumentation();

        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Register CurrentUser service to access the current user's information
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddJwtAuthentication();

        return services;
    }

    /// <summary>
    /// Creates a BadRequestObjectResult with a standardized error response for validation errors.
    /// It is needed because the default behavior of ASP.NET Core is to return a ProblemDetails response, which is not consistent with the rest of the API's error responses.
    /// </summary>
    private static BadRequestObjectResult CreateValidationErrorResponse(ActionContext context) 
    {
        var errors = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => error.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Validation failed.",
            Errors = errors,
            TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        });
    }
}
