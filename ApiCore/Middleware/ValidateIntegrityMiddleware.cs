using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace ApiCore.Middleware;
/// <summary>
/// Constructor
/// </summary>
/// <param name="logger"></param>
/// <param name="appSettings"></param>
/// <param name="rsaSecurity"></param>
/// <param name="_pluginFactory"></param>
public class ValidateIntegrityMiddleware(
    ILogger<ValidateIntegrityMiddleware> logger,
    IOptions<AppSetting> appSettings,
    RequestDelegate next) : MiddlewareBase(next, logger)
{
    private readonly AppSetting _appSettings = appSettings.Value;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var contextRequest = httpContext.Items[nameof(ContextRequest)] as ContextRequest;
        var integrityMode = _appSettings.IntegrityValidation;
        if (integrityMode.Enable && (!integrityMode.PathsExclude?.Any(t => t.Equals(httpContext.Request.Path.ToString(), StringComparison.CurrentCultureIgnoreCase)) ?? true))
        {
            try
            {
                ValidateHeaders(contextRequest);
                //Arma el nounce

                string bodyAsText = await GetRawBodyRequest(httpContext);
                string hashBody = string.IsNullOrEmpty(bodyAsText) ? string.Empty : bodyAsText.ToSha256();
                string queryString = GetQueryParameters(httpContext);
                var nounce = $"{httpContext.Request.Method}||{queryString}||{hashBody}||{contextRequest.Headers?.Time}";
                //Encripta
                var secretDecode = contextRequest.Headers.Secret.Decode();
                var secretDecrypt = RsaSecurity.Decrypt(_appSettings.RsaSecurity.ServerCertificateBase64PrivateKey, secretDecode);
                //Calcula el hash de integridad
                var hmacToken = nounce.ToSha256(secretDecrypt);
                //Comparar el hash
                await ValidateIntegrity(httpContext, contextRequest, nounce, secretDecrypt, hmacToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                    Logger.LogError(ex, "{@Message}", ex.Message);
                if (_appSettings.IntegrityValidation.ThrowExceptionIfError)
                    throw new CustomException(MessageCodes.SystemError, "Error al validar la integridad de datos");
            }
        }
        await Next(httpContext).ConfigureAwait(false);
    }

    /// <summary>
    /// Valida la integridad de los datos
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="contextRequest"></param>
    /// <param name="nounce"></param>
    /// <param name="secretDecrypt"></param>
    /// <param name="hmacToken"></param>
    /// <returns></returns>
    private async Task ValidateIntegrity(HttpContext httpContext, ContextRequest contextRequest, string nounce, string secretDecrypt, string hmacToken)
    {
        if (hmacToken != contextRequest.Headers?.Content)
        {
            var body = await GetRequestBodyAsync(httpContext.Request).ConfigureAwait(false);
            if (Logger.IsEnabled(LogLevel.Critical))
                Logger.LogCritical("El hash de integridad de datos del request:'{@XContent}' es distinto al calculado: '{@Hmac}'" +
                                 "'Nounce': {@Nounce} " +
                                 "'Secret': {@SecretKey} " +
                                 "'Headers': {@Headers} " +
                                 "'Body': {@Body} " +
                                 "'Context:':{@Context} ",
                                 contextRequest.Headers?.Content,
                                  hmacToken,
                                  nounce,
                                  secretDecrypt,
                                  GetHeaders(httpContext.Request),
                                  body,
                                  contextRequest);
            throw new CustomException(MessageCodes.ErrorIntegrity, "Error comparando la integridad de datos");
        }
    }

    /// <summary>
    /// Valida que estén presentes el resto de headers
    /// </summary>
    /// <param name="contextRequest"></param>
    private static void ValidateHeaders(ContextRequest contextRequest)
    {
        //Valida que estén presentes el resto de headers
        if (string.IsNullOrEmpty(contextRequest.Headers?.Content))
            throw new CustomException(MessageCodes.AuthorizationGeneric, "No esta presente el header X-Content");
        if (string.IsNullOrEmpty(contextRequest.Headers?.Time))
            throw new CustomException(MessageCodes.AuthorizationGeneric, "No esta presente el header X-Time");
        if (string.IsNullOrEmpty(contextRequest.Headers?.Secret))
            throw new CustomException(MessageCodes.AuthorizationGeneric, "No esta presente el header X-Secret");
    }

    /// <summary>
    /// Obtener cuerpo del request
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    protected static async Task<string> GetRawBodyRequest(HttpContext httpContext)
    {
        //Copia el Request para tomarlo como texto
        var bodyAsText = string.Empty;
        if (httpContext.Request.Body is null)
            return bodyAsText;
        httpContext.Request.EnableBuffering();
        httpContext.Request.Body.Position = 0;
        using var reader = new StreamReader(
            httpContext.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);
        bodyAsText = await reader.ReadToEndAsync();
        httpContext.Request.Body.Position = 0;
        return bodyAsText;
    }

    /// <summary>
    /// Obtener header
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="headerName"></param>
    /// <returns></returns>
    protected static KeyValuePair<string, StringValues> GetHeaderValue(HttpContext httpContext, string headerName) =>
        httpContext.Request.Headers.FirstOrDefault(p => p.Key.Equals(headerName, StringComparison.InvariantCultureIgnoreCase));

    /// <summary>
    /// Obtener query string
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    protected static string GetQueryParameters(HttpContext httpContext)
    {
        var queryString = string.Empty;
        if (httpContext.Request.QueryString.HasValue)
            queryString = httpContext.Request.QueryString.ToString().Replace("?", "");
        return queryString;
    }

    /// <summary>
    /// Obtiene el Body como un string 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        using StreamReader reader = new(request.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Obtiene el Body como un string 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static Dictionary<string, string> GetHeaders(HttpRequest request)
        => request.Headers.ToDictionary(h => h.Key, h => $"{h.Value}");
}