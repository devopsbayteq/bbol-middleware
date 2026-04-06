using ApiCore.Attributes;
using ApiCore.Models;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace ApiCore.Controllers;

[ServiceFilter(typeof(InjectContextAttribute))]
[ApiController]
public class ApiControllerBase(IMediator mediator) : ControllerBase
{
    protected readonly IMediator Mediator = mediator;

    /// <summary>
    /// Respuesta correcta
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected IActionResult Success<T>(T data)
    {
        var message = MessageCodes.Success.GetEnumMember();
        return Ok(new GenericResponse<T>
        {
            Code = (int)MessageCodes.Success,
            ResponseType = nameof(ResponseType.Success),
            Message = message,
            Content = data
        });
    }
}

