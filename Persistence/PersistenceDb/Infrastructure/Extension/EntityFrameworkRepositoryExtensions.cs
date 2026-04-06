using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistenceDb.Infrastructure.Modules;
using PersistenceDb.Models.Configuration;

namespace PersistenceDb.Infrastructure.Extension;

public static class EntityFrameworkRepositoryExtensions
{
    public static IServiceCollection AddUnitOfWorkRepository(this IServiceCollection services)
    {
        services
            .AddAuthenticationRepositories()
            .AddCoreRepositories()
            .AddUnitOfWorkManager();
        return services;
    }

    public static void AddCustomDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfiguration = configuration.GetSection("CustomConnectionStrings").Get<List<DatabaseConfiguration>>().FirstOrDefault()
            ?? throw new InvalidOperationException("No se encontró la configuración de la base de datos en el appsettings.json");
        services.AddPooledDbContextFactory<PersistenceContext>((serviceProvider, options) =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            options.UseSqlServer(databaseConfiguration.ConnectionString, optionBuilder =>
            {
                optionBuilder.UseNetTopologySuite();
                optionBuilder.CommandTimeout(databaseConfiguration.CommandTimeOut);
            })
            .UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging(databaseConfiguration.EnableSensitiveDataLogging)
            .EnableDetailedErrors(databaseConfiguration.EnableDetailedErrors);
        }
        );
    }
}