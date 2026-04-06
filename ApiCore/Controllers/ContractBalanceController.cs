using ApiCore.Models;
using Asp.Versioning;
using LogicApi.Model.Request.ContractBalance;
using LogicApi.Model.Response.ContractBalance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCore.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ContractBalanceController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Obtiene el saldo de un contrato
    /// </summary>
    /// <param name="request">Parámetros de consulta</param>
    /// <returns>Saldo del contrato</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<GetContractBalanceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance([FromQuery] GetContractBalanceRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Obtiene el home de productos del cliente
    /// </summary>
    /// <param name="request">Request vacio con contexto</param>
    /// <returns>Resumen de cuentas, tarjetas, prestamos, inversiones y pagos frecuentes</returns>
    [HttpGet("home")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<GetHomeDashboardResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHome([FromQuery] GetHomeDashboardRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));
}
