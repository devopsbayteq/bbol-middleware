using Serilog;
using Serilog.Core;
namespace ApiCore.Config;
public static class SerilogExtension
{
    /// <summary>
    /// Agrega Serilog a la configuración
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static Logger AddSerilogCustom(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        Log.Logger = logger;
        builder.Host.UseSerilog();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);
        builder.Services.AddCustomApplicationInsightsTelemetry(builder.Configuration);
        return logger;
    }

    /// <summary>
    /// Agrega configuración de Application Insigths Telemetry
    /// </summary>
    /// <param name="servicesCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomApplicationInsightsTelemetry(this IServiceCollection servicesCollection, IConfiguration configuration)
    {
        var applicationInsights = configuration.GetSection("Serilog:WriteTo")
            ?.Get<List<WriteTo>>()
            ?.Find(where => where.Name == "ApplicationInsights");
        if (applicationInsights is not null)
            servicesCollection.AddApplicationInsightsTelemetry(config => config.ConnectionString = applicationInsights.Args.ConnectionString ?? throw new InvalidOperationException("No existe cadena de conexión para ApplicationInsights"));
        return servicesCollection;
    }

    /// <summary>
    /// Obtiene un Log para registrar cualquier error en el inicio de la aplicación
    /// </summary>
    /// <returns></returns>
    public static Logger GetLoggerCritical()
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File("LogError/ApiCoreLog.log")
            .CreateLogger();
        return logger;
    }

}

/// <summary>
/// Configuraciones
/// </summary>
public class WriteTo
{
    /// <summary>
    /// Nombre
    /// </summary>
    /// <value></value>
    public string Name { get; set; }

    /// <summary>
    /// Argumentos
    /// </summary>
    /// <value></value>
    public Args Args { get; set; }
}

/// <summary>
/// Argumentos
/// </summary>
public class Args
{
    /// <summary>
    /// /// Cadena de conexión
    /// </summary>
    /// <value></value>
    public string ConnectionString { get; set; }
}
