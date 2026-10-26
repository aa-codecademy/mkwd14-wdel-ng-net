using Microsoft.AspNetCore.Authentication.JwtBearer;
using PizzaApp.Api.OpenApi;
using Scalar.AspNetCore;

namespace PizzaApp.Api.Extensions;

public static class OpenApiExtensions
{
    /// <summary>Describes the API as an OpenAPI document, including the JWT "Bearer" login.</summary>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
        return services;
    }

    /// <summary>/openapi/v1.json (the API description) and the Scalar UI at /scalar to try the API.</summary>
    public static WebApplication MapApiDocs(this WebApplication app)
    {
        // The docs are public: without AllowAnonymous the fallback policy would ask for a token
        app.MapOpenApi().AllowAnonymous();

        app.MapScalarApiReference(options => options
                .WithTitle("PizzaApp API")
                .AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme))
            .AllowAnonymous();

        return app;
    }
}
