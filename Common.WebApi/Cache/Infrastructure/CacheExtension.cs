using Common.WebApi.Cache.Implementation.DotNet;
using Common.WebApi.Cache.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Common.WebApi.Cache.Infrastructure;

public static class CacheExtension
{
    /// <summary>
    /// Registra caché en memoria y <see cref="IAdministratorCache"/> en el contenedor DI de .NET.
    /// </summary>
    public static IServiceCollection AddCustomCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IAdministratorCache, DotNetAdministratorCache>();
        return services;
    }
}
