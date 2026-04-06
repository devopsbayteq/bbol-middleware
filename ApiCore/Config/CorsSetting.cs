namespace ApiCore.Config;

public static class CorsExtension
{
    /// <summary>
    /// Aplica la política CORS registrada en <see cref="AddCorsSetting"/> (mismo nombre que <c>Cors:NamePolicy</c>).
    /// </summary>
    public static void UseCorsSetting(this IApplicationBuilder app, IConfiguration configuration)
    {
        var namePolicy = configuration["Cors:NamePolicy"];
        if (string.IsNullOrWhiteSpace(namePolicy))
            throw new InvalidOperationException("Configuración requerida: Cors:NamePolicy.");

        app.UseCors(namePolicy);
    }

    /// <summary>
    /// Lista blanca de orígenes vía <c>Cors:Origins</c> (dominios de confianza explícitos; sin comodines ni <c>AllowAnyOrigin</c>).
    /// </summary>
    public static void AddCorsSetting(this IServiceCollection services, IConfiguration configuration)
    {
        var namePolicy = configuration["Cors:NamePolicy"];
        if (string.IsNullOrWhiteSpace(namePolicy))
            throw new InvalidOperationException("Configuración requerida: Cors:NamePolicy.");

        var origins = (configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
            .Where(static o => !string.IsNullOrWhiteSpace(o))
            .Select(static o => o.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (origins.Length == 0)
        {
            throw new InvalidOperationException(
                "CORS: defina Cors:Origins con uno o más orígenes de confianza (URLs completas, p. ej. https://app.ejemplo.com).");
        }

        services.AddCors(options =>
        {
            options.AddPolicy(
                namePolicy,
                policy => policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
        });
    }
}
