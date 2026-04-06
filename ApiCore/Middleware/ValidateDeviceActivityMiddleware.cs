

using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using Microsoft.Extensions.Options;

namespace ApiCore.Middleware;

public class ValidateDeviceActivityMiddleware(
    ILogger<ValidateDeviceActivityMiddleware> logger,
    RequestDelegate next,
    IOptions<AppSetting> appSettings)
    : MiddlewareBase(next, logger)
{

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var contextRequest = httpContext.Items[nameof(ContextRequest)] as ContextRequest;
        var platform = contextRequest.Headers.Platform;
        //Obtiene la configuración de validación de actividad de dispositivo
        var deviceActivityStatusValidationConfig = appSettings.Value.DeviceActivityStatusValidationConfig
             ?? throw new CustomException(MessageCodes.SystemError, $"No se encuentra configuración de validación de actividad de dispositivo");
        //Obtiene la configuración de la plataforma
        var platformConfig = deviceActivityStatusValidationConfig.Platform
            .FirstOrDefaultValue(where => where.Key.Equals($"{platform}", StringComparison.CurrentCultureIgnoreCase)) ?? throw new CustomException(MessageCodes.NoDeviceActivityStatusValidationConfig, $"No se encuentra configuración de validación de actividad de dispositivo para la plataforma '{platform}'");
        //Obtiene si el path actual está excluido
        var currentPathIsExcluded = deviceActivityStatusValidationConfig.ExcludePaths
            ?.Exists(where => where.Method.Equals(httpContext.Request.Method, StringComparison.CurrentCultureIgnoreCase)
          && where.Path.Equals(httpContext.Request.Path, StringComparison.CurrentCultureIgnoreCase)) ?? false;
        //Obtiene el estado de la actividad del dispositivo
        if (platformConfig.Enable && !currentPathIsExcluded)
        {
            //Obtiene el estado de la actividad del dispositivo
            var deviceStateHeader = GetFingerPrintingModel(httpContext, appSettings.Value.RsaSecurity.ServerBase64PrivateKey).DeviceState;
            if (!platformConfig.AllowedStatus.Exists(where => where.Equals(deviceStateHeader, StringComparison.CurrentCultureIgnoreCase)))
            {
                var deviceId = contextRequest.Headers.DeviceId;
                if (logger.IsEnabled(LogLevel.Critical))
                    logger.LogCritical("Alerta!, El dispositivo con Plataforma {@DevicePlatform}, Identificador {@Device} y Usuario {@User} intentó ejecutar el servicio {@ServiceMethod} {@ServicePath} con el estado {@DeviceState} que no es permitido.",
                         $"{platform}",
                         deviceId,
                         contextRequest.CustomClaims.UserId,
                         httpContext.Request.Path.Value,
                         httpContext.Request.Method,
                         deviceStateHeader
                         );
                throw new CustomException(MessageCodes.DeviceStateNotAllowed, $"El estado de la actividad del dispositivo es inválido: {deviceStateHeader}");
            }
        }
        await Next(httpContext).ConfigureAwait(false);
    }


}