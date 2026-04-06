namespace LogicApi.Model.Response.Security;

/// <summary>
/// Respuesta para validacion de monto de transaccion
/// </summary>
public class ValidateTransactionAmountResponse
{
    /// <summary>
    /// Indica si el monto es valido
    /// </summary>
    public bool IsValid { get; set; }
}
