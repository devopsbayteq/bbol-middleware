using ApiCore.Models;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Security;
using Microsoft.Extensions.Primitives;

namespace ApiCore.Middleware;

/// <summary>
/// Constructor
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
public abstract class MiddlewareBase(
    RequestDelegate next,
    ILogger<MiddlewareBase> logger
)
{
    protected readonly RequestDelegate Next = next;
    protected readonly ILogger<MiddlewareBase> Logger = logger;

    /// <summary>
    /// Obtiene un Header
    /// </summary>
    /// <param name="headerName"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected static KeyValuePair<string, StringValues> GetHeaderFromName(string headerName, HttpContext context)
        => context.Request.Headers.FirstOrDefault(p => p.Key.Equals(headerName, StringComparison.InvariantCultureIgnoreCase));


    /// <summary>
    /// Obtiene un Header
    /// </summary>
    /// <param name="headerName"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected static string GetHeaderByName(string headerName, HttpContext context)
        => GetHeaderFromName(headerName, context).Value;

    /// <summary>
    /// Obtiene el modelo de fingerprinting
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    protected static FingerPrintingModel GetFingerPrintingModel(HttpContext httpContext, string privateServerKey)
    {
        if (httpContext.Items[nameof(FingerPrintingModel)] is not FingerPrintingModel fingerPrintingModel)
        {
            var xFingerprint = GetHeaderByName("X-Fingerprint", httpContext);
            if (string.IsNullOrEmpty(xFingerprint))
                throw new CustomException(MessageCodes.HeaderNotFound, "El header X-Fingerprint es requerido");
            var fingerprintDecrypt = RsaSecurity.Decrypt(privateServerKey, xFingerprint.Decode());
            fingerPrintingModel = fingerprintDecrypt.ToObject<FingerPrintingModel>();
            httpContext.Items[nameof(FingerPrintingModel)] = fingerPrintingModel;
        }
        return fingerPrintingModel;
    }

}
