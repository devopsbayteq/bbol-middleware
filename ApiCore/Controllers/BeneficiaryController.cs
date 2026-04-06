using ApiCore.Models;
using Asp.Versioning;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Response.Beneficiary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCore.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class BeneficiaryController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Lista de contactos beneficiarios
    /// </summary>
    /// <param name="request">Request vacio con contexto</param>
    /// <returns>Lista de beneficiarios</returns>
    [HttpGet("contacts")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<GetBeneficiaryContactsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContacts([FromQuery] GetBeneficiaryContactsRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));
}
