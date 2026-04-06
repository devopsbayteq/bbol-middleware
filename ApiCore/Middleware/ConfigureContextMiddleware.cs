using System.Security.Claims;
using Common.Utils.Extensions;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Clock;
using Common.WebApi.Security;
using Microsoft.Extensions.Options;
namespace ApiCore.Middleware;

public class ConfigureContextMiddleware(
    ILogger<ConfigureContextMiddleware> logger,
    RequestDelegate next,
    IOptions<AppSetting> appSettings
        ) : MiddlewareBase(next, logger)
{
    private readonly string _aesSecret = appSettings.Value.AesSecurity.Key;

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext, IClock clock)
    {
        //Toma los headers de interés
        var xRequestId = GetHeaderFromName("X-RequestId", httpContext);
        var xplatform = GetHeaderFromName("X-Platform", httpContext);
        var xversion = GetHeaderFromName("X-Version", httpContext);
        var xtime = GetHeaderFromName("X-Time", httpContext);
        var deviceId = GetHeaderFromName("X-Device", httpContext);
        var xmodel = GetHeaderFromName("X-Model", httpContext);
        var xbrand = GetHeaderFromName("X-Brand", httpContext);
        var xTimezone = GetHeaderFromName("X-Timezone", httpContext);
        var xContent = GetHeaderFromName("X-Content", httpContext);
        var xSecret = GetHeaderFromName("X-Secret", httpContext);
        var authorization = GetHeaderFromName("Authorization", httpContext);
        var xSystemOperationVersion = GetHeaderFromName("X-SystemOperationVersion", httpContext);

        // Configura la zona horaria por request para consumidores de IClock.
        clock.ConfigureTimeZone(xTimezone.Value);

        if (!Enum.TryParse<PlatformType>(xplatform.Value, ignoreCase: true, out var platform))
            throw new CustomException(MessageCodes.SystemError, $"La plataforma es invalida (X-Platform: {xplatform.Value}).");

        if (!long.TryParse(xtime.Value, out var timestamp))
            throw new CustomException(MessageCodes.SystemError, $"El timestamp tiene un formato invalido (X-Time: {xtime.Value}).");

        // Claim por tipo
        var claimEncryptedFieldClaim = httpContext.User?.FindFirst(nameof(EncryptedFieldClaim))?.Value;

        //Obtiene los valores encriptados
        var encryptedFieldClaim = GetEncryptedFieldClaim(claimEncryptedFieldClaim);

        //Arma el contexto de auditoría
        httpContext.Items[nameof(ContextRequest)] =
            new ContextRequest
            {
                RequestId = xRequestId.Value,
                Headers = new()
                {
                    Model = xmodel.Value,
                    Brand = xbrand.Value,
                    VersionApplication = xversion.Value,
                    Platform = platform,
                    DeviceId = deviceId.Value,
                    TimeZone = xTimezone.Value,
                    Content = xContent.Value,
                    Secret = xSecret.Value,
                    Time = xtime.Value,
                    Authorization = authorization.Value,
                    VersionSystemOperation = xSystemOperationVersion.Value,
                    UnixTime = timestamp
                },
                CustomClaims = new CustomClaims
                {
                    UserId = encryptedFieldClaim?.UserGuid,
                    UserName = encryptedFieldClaim?.UserName,
                    DeviceGuid = encryptedFieldClaim?.DeviceId,
                },
            };
        await Next(httpContext).ConfigureAwait(false);
    }

   
    /// <summary>
    /// Desencripta los datos enviados
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private EncryptedFieldClaim GetEncryptedFieldClaim(string encryptedFieldClaim)
    {
        if (encryptedFieldClaim.IsNullOrEmpty())
            return null;
        var encryptedFieldClaimJson = AesSecurity.DecryptAes(encryptedFieldClaim, _aesSecret);
        return encryptedFieldClaimJson.ToObject<EncryptedFieldClaim>();
    }
}
