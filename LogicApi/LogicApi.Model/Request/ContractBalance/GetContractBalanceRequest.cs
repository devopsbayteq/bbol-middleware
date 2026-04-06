using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.ContractBalance;

namespace LogicApi.Model.Request.ContractBalance;

/// <summary>
/// Solicitud para consultar el saldo de un contrato
/// </summary>
public class GetContractBalanceRequest : IApiBaseRequest<GetContractBalanceResponse>
{
    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; } 
}
