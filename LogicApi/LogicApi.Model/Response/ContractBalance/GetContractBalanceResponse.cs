namespace LogicApi.Model.Response.ContractBalance;

/// <summary>
/// Respuesta con el saldo asociado a un contrato
/// </summary>
public class GetContractBalanceResponse
{
    /// <summary>
    /// Número de contrato
    /// </summary>
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>
    /// Saldo disponible
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Código de moneda (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = string.Empty;
}
