using System.Diagnostics;

namespace ApiCore.Middleware;

/// <summary>
/// Constructor
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
/// <param name="pluginFactory"></param>
public class TimeDurationRequestMiddleware(
    RequestDelegate next,
    ILogger<TimeDurationRequestMiddleware> logger
        ) : MiddlewareBase(next, logger)
{

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Restart();
            await Next(httpContext).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("Request: {@TraceIdentifier} - Tiempo de Respuesta: {@ResponseTime}ms", httpContext.TraceIdentifier, stopwatch.ElapsedMilliseconds);
        }
    }
}
