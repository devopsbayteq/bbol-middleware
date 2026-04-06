namespace LogicApi.Model.Response.Transaction;

/// <summary>
/// Respuesta de transacciones recientes
/// </summary>
public class GetRecentTransactionsResponse
{
    /// <summary>
    /// Lista de transacciones recientes
    /// </summary>
    public List<TransactionItem> Items { get; set; } = [];
}
