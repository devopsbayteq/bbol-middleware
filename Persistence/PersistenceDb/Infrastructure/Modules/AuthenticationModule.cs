using Microsoft.Extensions.DependencyInjection;
using PersistenceDb.Authentication;
using PersistenceDb.Repository.Interfaces.Authentication;

namespace PersistenceDb.Infrastructure.Modules;

public static class AuthenticationModule
{
    public static IServiceCollection AddAuthenticationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}