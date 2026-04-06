using Common.WebApi.Models.AppSettings;
using Common.WebApi.Clock;
using Microsoft.Extensions.Options;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.Utils.Extensions;
namespace ApiCore.Middleware;

public class ValidateClientTimeMiddleware(
    RequestDelegate next,
    ILogger<ValidateClientTimeMiddleware> logger,
    IOptions<AppSetting> appSettings
        ) : MiddlewareBase(next, logger)
{
    public async Task InvokeAsync(HttpContext httpContext, IClock clock)
    {
        var xTime = GetHeaderByName("X-Time", httpContext);
        if (string.IsNullOrEmpty(xTime))
            throw new CustomException(MessageCodes.HeaderNotFound, "El header X-Time es requerido");
        if (!long.TryParse(xTime, out var timestamp))
            throw new CustomException(MessageCodes.HeaderFormatInvalid, "El header X-Time tiene un formato invalido");
        var dateTime = timestamp.ToDateTimeOffSet().DateTime;
        var timeDifference = dateTime - clock.UtcNow();
        if (timeDifference.TotalSeconds > appSettings.Value.Time.TimeToleranceSeconds)
            throw new CustomException(MessageCodes.ClientTimeOutdated, $"El header X-Time ({dateTime}) tiene una diferencia de {timeDifference.TotalSeconds} segundos con la fecha actual ({clock.Now()}) con tolerancia de {appSettings.Value.Time.TimeToleranceSeconds} segundos");
        await Next(httpContext).ConfigureAwait(false);
    }
}
