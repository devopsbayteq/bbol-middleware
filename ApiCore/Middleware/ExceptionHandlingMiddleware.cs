
using System.Net;
using ApiCore.Models;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.Enum;

namespace ApiCore.Middleware;

/// <summary>
/// Constructor
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
/// <param name="pluginFactory"></param>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
) : MiddlewareBase(next, logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await Next(httpContext).ConfigureAwait(false);
        }
        catch (CustomException ex)
        {
            var message = ex.MessageCode.GetEnumMember();
            var messageResponse = $"{message} ({(int)ex.MessageCode})";
            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation(ex, "CustomException (Code: {@Code} - HTTP: {@CodeHttp} - Message: {@Message} - Reason: {@AdditionalInfoError})", ex.MessageCode, (int)HttpStatusCode.OK, message, ex.Message);
            await SetMessageResponse(httpContext, (int)HttpStatusCode.OK, (int)ex.MessageCode, messageResponse, null, ex.Message);
        }
        catch (Exception ex)
        {
            var code = (int)MessageCodes.SystemError;
            Logger.LogError(ex, "Exception (HTTP: {@CodeHttp} - {@Message}) ", (int)HttpStatusCode.InternalServerError, ex.Message);
            await SetMessageResponse(httpContext, (int)HttpStatusCode.InternalServerError, code, MessageCodes.SystemError.GetEnumMember(), (string)null, ex.Message).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Controlador Genérico 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="customException"></param>
    /// <returns></returns>
    private async Task SetMessageResponse<T>(
        HttpContext context,
        int httpCode,
        int codeResponse,
        string messageResponse,
        string additionalData,
        T content)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = httpCode;
        var response = new GenericResponse<T>
        {
            Code = codeResponse,
            ResponseType = nameof(ResponseType.Error),
            Message = messageResponse,
            Content = content,
            AdditionalData = additionalData
        }.ToString();
        Logger.LogError("Response Error: {@Path} {@Method} : {@Model}", context.Request.Path.Value, context.Request.Method, response);
        await context.Response.WriteAsync(response).ConfigureAwait(false);
    }

}
