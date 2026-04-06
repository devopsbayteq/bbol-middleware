using ApiCore.Models;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using Microsoft.Extensions.Options;


namespace ApiCore.Middleware;

public class ValidateRootDeviceMiddleware(
    ILogger<ValidateRootDeviceMiddleware> logger,
    RequestDelegate next,
    IOptions<AppSetting> appSettings)
    : MiddlewareBase(next, logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var contextRequest = httpContext.Items[nameof(ContextRequest)] as ContextRequest;
        var platform = contextRequest.Headers.Platform;
        //Obtiene la configuración de validación de actividad de dispositivo
        var deviceRootValidationConfig = appSettings.Value.DeviceRootValidationConfig
             ?? throw new CustomException(MessageCodes.SystemError, $"No se encuentra configuración de validación de root de dispositivo");
        //Obtiene la configuración de la plataforma
        var platformConfig = deviceRootValidationConfig.Platform
            .FirstOrDefaultValue(where => where.Key.Equals($"{platform}", StringComparison.CurrentCultureIgnoreCase)) ?? throw new CustomException(MessageCodes.NoDeviceRootValidationConfig, $"No se encuentra configuración de validación de root de dispositivo para la plataforma '{platform}'");
        //Obtiene si el path actual está excluido
        var currentPathIsExcluded = deviceRootValidationConfig.ExcludePaths
            ?.Exists(where => where.Method.Equals(httpContext.Request.Method, StringComparison.CurrentCultureIgnoreCase)
          && where.Path.Equals(httpContext.Request.Path, StringComparison.CurrentCultureIgnoreCase)) ?? false;
        //Obtiene el estado de la actividad del dispositivo
        if (platformConfig.Enable && !currentPathIsExcluded)
            ValidateRootDevice(httpContext, contextRequest, platform, platformConfig);
        await Next(httpContext).ConfigureAwait(false);
    }

    /// <summary>
    /// Valida si el dispositivo es root
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="contextRequest"></param>
    /// <param name="platform"></param>
    /// <param name="platformConfig"></param>
    private void ValidateRootDevice(HttpContext httpContext, ContextRequest contextRequest, PlatformType platform, DeviceRootValidationConfig.DeviceRootValidationPlatformConfig platformConfig)
    {
        var fingerPrintingModel = GetFingerPrintingModel(httpContext, appSettings.Value.RsaSecurity.ServerBase64PrivateKey);
        //Obtiene el modelo de fingerprinting
        try
        {
            if (!platformConfig.AllowRoot && fingerPrintingModel.IsRoot)
                throw new CustomException(MessageCodes.DeviceRootNotAllowed, $"El dispositivo se encuentra en modo Root");
            if (!platformConfig.AllowDebugger && fingerPrintingModel.IsDebugger)
                throw new CustomException(MessageCodes.DeviceDebuggerNotAllowed, $"El dispositivo se encuentra en modo Debugger");
            if (!platformConfig.AllowDevelopmentMode && fingerPrintingModel.IsDevelopment)
                throw new CustomException(MessageCodes.DeviceDevelopmentNotAllowed, $"El dispositivo se encuentra en modo Development");
            if (!platformConfig.AllowEmulatorDevice && !fingerPrintingModel.IsPhysicalDevice)
                throw new CustomException(MessageCodes.EmulatorDeviceNotAllowed, $"El dispositivo está en modo Emulador");
        }
        catch (Exception ex)
        {
            var deviceId = contextRequest.Headers.DeviceId;
            if (logger.IsEnabled(LogLevel.Critical))
                logger.LogCritical(
                    ex,
                    "Alerta!, El dispositivo con Plataforma {@DevicePlatform}, Identificador {@Device} y Usuario {@User} intentó ejecutar el servicio {@ServiceMethod} {@ServicePath} con una configuración no permitida. {@Message}",
                    $"{platform}",
                    deviceId,
                    contextRequest.CustomClaims.UserId,
                    httpContext.Request.Method,
                    httpContext.Request.Path.Value,
                    ex.Message);
            throw;
        }
    }
}