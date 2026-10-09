using PizzaApp.Api.ExceptionHandling;

namespace PizzaApp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                // TODO : implement custom behavior for model validation errors
            });

        services.AddOpenApi();
        services.AddOpenApiDocumentation();

        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddJwtAuthentication();

        return services;
    }
}
