using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using PizzaApp.Dtos.Common;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Api.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        int statusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        bool isUnexpected = statusCode == StatusCodes.Status500InternalServerError;
        if (isUnexpected)
        {
            // log
        }

        ErrorResponse response = new()
        {
            StatusCode = statusCode,
            Message = isUnexpected ? "An unexpected error occurred." : exception.Message,
            Errors = exception is BadRequestException badRequest ? [.. badRequest.Errors] : [],
            //Errors = exception is BadRequestException
            //    ? ((BadRequestException)exception).Errors
            //    : null,
            TraceId = Activity.Current?.Id ?? httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
