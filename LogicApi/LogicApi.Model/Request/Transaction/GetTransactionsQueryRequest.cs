using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Enums;
using LogicApi.Model.Response.Transaction;

namespace LogicApi.Model.Request.Transaction;

/// <summary>
/// Solicitud para consultar transacciones por filtros
/// </summary>
public class GetTransactionsQueryRequest : IApiBaseRequest<GetTransactionsQueryResponse>
{
    /// <summary>
    /// Guid de cuenta a consultar
    /// </summary>
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// Fecha desde (opcional)
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Fecha hasta (opcional)
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Tipo de transaccion (opcional)
    /// </summary>
    public List<TransactionType> TransactionTypes { get; set; } = [];

    /// <summary>
    /// Monto maximo (opcional)
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Monto maximo (opcional)
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Texto de busqueda (opcional)
    /// </summary>
    public string TextSearch { get; set; }

    /// <summary>
    /// Pagina actual
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Registros por pagina
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
