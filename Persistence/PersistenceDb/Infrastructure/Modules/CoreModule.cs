using Microsoft.Extensions.DependencyInjection;
using PersistenceDb.Core;
using PersistenceDb.Repository.Interfaces.Core;

namespace PersistenceDb.Infrastructure.Modules;

public static class CoreModule
{
    public static IServiceCollection AddCoreRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountUserRepository, AccountUserRepository>();
        services.AddScoped<IUserDeviceChallengeRepository, UserDeviceChallengeRepository>();
        services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        return services;
    }
}