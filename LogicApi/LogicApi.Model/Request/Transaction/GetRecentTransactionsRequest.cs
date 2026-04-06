using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Transaction;

namespace LogicApi.Model.Request.Transaction;

/// <summary>
/// Solicitud para obtener transacciones recientes
/// </summary>
public class GetRecentTransactionsRequest : IApiBaseRequest<GetRecentTransactionsResponse>
{
    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
