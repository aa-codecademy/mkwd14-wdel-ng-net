using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace PizzaApp.Mappers;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Mapster: every <see cref="IRegister"/> mapping config in this project, and <see cref="IMapper"/>.
    /// </summary>
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Every mapping must be declared in a config class (like CreateMap in AutoMapper):
        // mapping two types nobody configured is an error, not a silent guess
        config.RequireExplicitMapping = true;

        // Finds every class in this project that implements IRegister (PizzaMappingConfig, OrderMappingConfig, ...)
        config.Scan(typeof(DependencyInjection).Assembly);

        // Fail fast: a broken mapping crashes the app at startup, not in the middle of a request
        config.Compile();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
