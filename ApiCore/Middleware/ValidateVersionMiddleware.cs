using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using Microsoft.Extensions.Options;

namespace ApiCore.Middleware;
/// <summary>
/// Constructor
/// </summary>
/// <param name="next"></param>
/// <param name="appSettings"></param>
public class ValidateVersionMiddleware(
    ILogger<ValidateVersionMiddleware> logger,
    IOptions<AppSetting> appSettings,
    RequestDelegate next)
     : MiddlewareBase(next, logger)
{

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        //Obtiene el contexto
        var contextRequest = httpContext.Items[nameof(ContextRequest)] as ContextRequest;
        var platformType = contextRequest.Headers.Platform;
        var currentPlatformConfigurationVersion = appSettings.Value.VersionsConfiguration?.Find(p => p.Platform == platformType);
        //Verifica si se registró un token
        if (currentPlatformConfigurationVersion?.Validate ?? false)
        {
            var version = GetHeaderByName("X-Version", httpContext);
            //Convierte la versión de los headers
            if (!Version.TryParse(version, out var versionMobile))
                throw new CustomException(MessageCodes.SystemError, $"No se puede parsear la versión del móvil: '{version}'");
            //Convierte la versión del appSettings
            if (!Version.TryParse(currentPlatformConfigurationVersion.Version, out var supportVersion))
                throw new CustomException(MessageCodes.SystemError, $"No se puede parsear la versión actual de configuración: '{currentPlatformConfigurationVersion.Version}'");
            if (versionMobile < supportVersion)
                throw new CustomException(MessageCodes.VersionNotAllowed, $"La versión mínima soportada es:'{currentPlatformConfigurationVersion.Version}'");
        }
        await Next(httpContext).ConfigureAwait(false);
    }
}
