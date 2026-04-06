using ApiCore.Models;
using Asp.Versioning;
using LogicApi.Model.Request.Authentication;
using LogicApi.Model.Response.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCore.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthenticationController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Login endpoint
    /// </summary>
    /// <param name="request">Login request</param>
    /// <returns>Login response</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Login biométrico
    /// </summary>
    /// <param name="request">Datos de login biométrico</param>
    /// <returns>Token de acceso</returns>
    [HttpPost("biometric-login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BiometricLogin([FromBody] BiometricLoginRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));
}
