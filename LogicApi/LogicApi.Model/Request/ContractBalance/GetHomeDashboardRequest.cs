using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.ContractBalance;

namespace LogicApi.Model.Request.ContractBalance;

/// <summary>
/// Solicitud para el home de productos
/// </summary>
public class GetHomeDashboardRequest : IApiBaseRequest<GetHomeDashboardResponse>
{
    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
