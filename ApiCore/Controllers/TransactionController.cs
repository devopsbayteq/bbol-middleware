using ApiCore.Attributes;
using ApiCore.Models;
using Asp.Versioning;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.Transaction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCore.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TransactionController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Obtiene transacciones recientes
    /// </summary>
    /// <param name="request">Request vacio con contexto</param>
    /// <returns>Lista de transacciones recientes</returns>
    [HttpGet("recent")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<GetRecentTransactionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromQuery] GetRecentTransactionsRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Crea una transferencia
    /// </summary>
    /// <param name="request">Datos de transferencia</param>
    /// <returns>Identificador de transferencia</returns>
    [HttpPost("transfer")]
    [Authorize]
    [ServiceFilter(typeof(ValidateDuplicateRequestAttribute))]
    [ProducesResponseType(typeof(GenericResponse<CreateTransferResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Consulta transacciones por cuenta con filtros y paginacion
    /// </summary>
    /// <param name="request">Filtros de consulta</param>
    /// <returns>Resultado paginado de transacciones</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<GetTransactionsQueryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionsQueryRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));
}
