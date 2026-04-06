using BankCore.Integration.Interfaces;
using BankCore.Integration.Models;
using BankCore.Integration.Security;
using BankCore.Integration.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankCore.Integration.Extensions;

public static class BankCoreServiceCollectionExtensions
{
    /// <summary>Separador de segmentos en rutas URI HTTP (RFC 3986), no confundir con separadores de sistema de archivos.</summary>
    private const char HttpUriPathSeparator = '/';

    public static IServiceCollection AddBankCoreIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(BankCoreOptions.SectionName);
        services.Configure<BankCoreOptions>(section);

        var options = section.Get<BankCoreOptions>() ?? new BankCoreOptions();

        services.AddHttpClient("BankCoreApiClient", c =>
        {
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                var baseWithoutTrailingSlash = options.BaseUrl.TrimEnd(HttpUriPathSeparator);
                c.BaseAddress = new Uri(string.Concat(baseWithoutTrailingSlash, HttpUriPathSeparator));
            }
            c.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient("BankCoreAuthClient", c =>
        {
            if (!string.IsNullOrWhiteSpace(options.AuthUrl))
                c.BaseAddress = new Uri(options.AuthUrl);
            c.Timeout = TimeSpan.FromSeconds(Math.Max(5, options.TokenRequestTimeoutSeconds));
        });

        services.AddSingleton<ITokenProvider, BankCoreTokenProvider>();

        if (options.UseMock)
            services.AddSingleton<IBankCoreServices, MockBankCoreServices>();
        else
            services.AddSingleton<IBankCoreServices, HttpsBankCoreServices>();

        return services;
    }
}
