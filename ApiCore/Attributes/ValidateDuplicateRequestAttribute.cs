
using Common.WebApi.Cache.Interface;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiCore.Attributes;
/// <summary>
/// Constructor
/// </summary>
/// <param name="pluginFactory"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

public class ValidateDuplicateRequestAttribute(
    IOptions<AppSetting> appSettings,
    ILogger<ValidateDuplicateRequestAttribute> logger,
    IAdministratorCache administratorCache
    ) : ActionFilterAttribute
{
    private readonly ILogger<ValidateDuplicateRequestAttribute> _logger = logger;

    /// <summary>
    /// Al ejecutar Request
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //Verifica que esté habilitado
        if (appSettings.Value?.ValidateDuplicateRequestConfig?.Enable ?? false)
            foreach (var parameterDescriptor in context.ActionDescriptor.Parameters.Where(where => where.ParameterType.GetInterfaces().Contains(typeof(IApiBaseRequest))))
            {
                var postObject = (IApiBaseRequest)context.ActionArguments[parameterDescriptor.Name];
                var contextRequest = context.HttpContext.Items[nameof(ContextRequest)] as ContextRequest;
                postObject.ContextRequest = contextRequest;
                var jsonRequest = JsonConvert.SerializeObject(postObject);
                var jsonRequestSha256 = jsonRequest.ToSha256();
                var actionName = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName;
                //desencripta
                var usernameDecrypt = contextRequest.CustomClaims.UserName;
                //Genera la Key de almacenamiento
                var key = $"{nameof(ValidateDuplicateRequestAttribute)}-{usernameDecrypt}-{actionName}";
                //Verifica si existe datos en la key del cache
                var listHashRegister = await administratorCache.TryGetOrSetAsync(
                    key,
                    async () => await Task.FromResult(new List<ValidationDuplicateRequest>()).ConfigureAwait(false),
                    (int)TimeSpan.FromMinutes(appSettings.Value.ValidateDuplicateRequestConfig.MinutesDurationCache).TotalSeconds).ConfigureAwait(false);
                if (listHashRegister.Exists(register => register.HashRequest == jsonRequestSha256))
                {
                    if (_logger.IsEnabled(LogLevel.Critical))
                        _logger.LogCritical("Se detectó un Request duplicado para el Usuario: '{@UserName}' y el Contenido: {@Content}", usernameDecrypt, jsonRequest);
                    throw new CustomException(MessageCodes.DuplicateRequest, "El request enviado ya ha sido registrado anteriormente.");
                }
                listHashRegister.Add(new ValidationDuplicateRequest(DateTime.UtcNow, jsonRequestSha256));
            }
        await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
    }
}

/// <summary>
/// Clase modelo para almacenar información
/// </summary>
public class ValidationDuplicateRequest
{
    /// <summary>
    /// Fecha de Registro
    /// </summary>
    /// <value></value>
    public DateTime DateTime { get; set; }


    /// <summary>
    /// Hash de Request
    /// </summary>
    /// <value></value>
    public string HashRequest { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="hashRequest"></param>
    public ValidationDuplicateRequest(DateTime dateTime, string hashRequest)
    {
        DateTime = dateTime;
        HashRequest = hashRequest;
    }

}