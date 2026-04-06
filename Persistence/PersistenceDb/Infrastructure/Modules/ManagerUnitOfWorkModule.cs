using Microsoft.Extensions.DependencyInjection;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using PersistenceDb.UnitOfWork;

namespace PersistenceDb.Infrastructure.Modules;

public static class ManagerUnitOfWorkModule
{
    public static IServiceCollection AddUnitOfWorkManager(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        services.AddScoped<IUnitOfWork, MainUnitOfWork>();
        return services;
    }
}