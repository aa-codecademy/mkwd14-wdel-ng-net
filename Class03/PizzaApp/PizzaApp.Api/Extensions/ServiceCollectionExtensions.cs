using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaApp.Api.ExceptionHandling;
using PizzaApp.Dtos.Common;

namespace PizzaApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
                options.InvalidModelStateResponseFactory = CreateValidationErrorResponse;
            });

        services.AddOpenApi();
        services.AddOpenApiDocumentation();

        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddJwtAuthentication();

        return services;
    }

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
