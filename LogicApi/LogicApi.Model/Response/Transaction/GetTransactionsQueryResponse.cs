namespace LogicApi.Model.Response.Transaction;

/// <summary>
/// Respuesta paginada de transacciones
/// </summary>
public class GetTransactionsQueryResponse
{
    /// <summary>
    /// Total de registros luego de filtros
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Pagina actual
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Registros por pagina
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Lista de transacciones
    /// </summary>
    public IEnumerable<TransactionItem> Items { get; set; } = [];
}
