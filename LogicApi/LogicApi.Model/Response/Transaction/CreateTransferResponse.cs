namespace LogicApi.Model.Response.Transaction;

/// <summary>
/// Respuesta al crear transferencia
/// </summary>
public class CreateTransferResponse
{
    /// <summary>
    /// Identificador de la transferencia
    /// </summary>
    public string TransactionIdentifier { get; set; } = string.Empty;
}
