using Microsoft.AspNetCore.Mvc.Filters;
using ApiCore.Models;
using Common.WebApi.Models.AppSettings;
using Microsoft.Extensions.Options;
using Common.WebApi.Models;
using Newtonsoft.Json;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Common.WebApi.Attributes.Json;
namespace ApiCore.Attributes;
/// <summary>
/// Agregar contexto en Api Gateway
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="appSettings"></param>
/// <param name="logger"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class InjectContextAttribute(ILogger<InjectContextAttribute> logger, IOptions<AppSetting> appSettings) : ActionFilterAttribute
{
    protected readonly ILogger<InjectContextAttribute> Logger = logger;
    protected readonly AppSetting AppSettings = appSettings.Value;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var parameterDescriptor in context.ActionDescriptor.Parameters)
        {
            if (!parameterDescriptor.ParameterType.GetInterfaces().Contains(typeof(IApiBaseRequest))) continue;
            var postObject = (IApiBaseRequest)context.ActionArguments[parameterDescriptor.Name];
            postObject.ContextRequest = context.HttpContext.Items[nameof(ContextRequest)] as ContextRequest;
            var header = GetHeaderCustom(context.HttpContext.Request);
            var method = context.HttpContext.Request.Method;
            var path = GetPath(context.HttpContext);
            var requestId = postObject.ContextRequest.RequestId;
            Task.Run(() =>
            {
                SaveLoggerRequestAsync(new LogRequestModel
                {
                    LogMessage = $"Request Headers: {requestId}",
                    ModelToLog = header,
                    Method = method,
                    Path = path,
                });
                SaveLoggerRequestAsync(new LogRequestModel
                {
                    LogMessage = $"Request Body: {requestId}",
                    ModelToLog = postObject,
                    Method = method,
                    Path = path,
                });
            });
        }
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        if (context.Result is OkObjectResult result)
        {
            var statusCode = context.HttpContext.Response.StatusCode;
            var path = GetPath(context.HttpContext);
            var method = context.HttpContext.Request.Method;
            Task.Run(() =>
                SaveLoggerRequestAsync(new LogRequestModel
                {
                    LogMessage = $"Response: {statusCode}",
                    ModelToLog = result.Value,
                    Path = path,
                    Method = method
                })
            );
        }
        else
        {
            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("Response not OkObjectResult: {@StatusCode}", context.HttpContext.Response.StatusCode);
        }
    }



    /// <summary>
    /// Obtiene el Path de del contexto
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    protected static string GetPath(HttpContext httpContext)
    {
        var queryParams = $"{httpContext.Request.QueryString}";
        var path = httpContext.Request.Path;
        var host = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        var urlComplete = $"{host}{path}";
        if (!string.IsNullOrEmpty(queryParams))
            urlComplete = $"{urlComplete}{queryParams}";
        return urlComplete;
    }

    /// <summary>
    /// Envia a Logear el Request 
    /// </summary>
    /// <param name="logRequestModel"></param>
    protected void SaveLoggerRequestAsync(LogRequestModel logRequestModel)
    {
        try
        {
            var jsonRequest = JsonConvert.SerializeObject(logRequestModel.ModelToLog,
            new JsonSerializerSettings
            {
                ContractResolver = AppSettings.LogSensitiveInformation ?
            new SensitiveDevelopmentProperty()
            : new SensitiveProductionProperty(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("{@LogMessage}: {@Path} {@Method} : {@Model}", logRequestModel.LogMessage, logRequestModel.Path, logRequestModel.Method, jsonRequest);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al registrar Log: {@Message}", ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los headers sin el Token
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected Dictionary<string, string> GetHeaderCustom(HttpRequest request)
    {
        var headersClone = request.Headers.ToDictionary(t => t.Key.ToUpper(), t => $"{t.Value}");
        if (!AppSettings.LogHeadersRemove.IsNullOrEmpty())
            foreach (var header in AppSettings.LogHeadersRemove.Select(t => t.ToUpper()))
                headersClone.Remove(header);
        return headersClone;
    }


}