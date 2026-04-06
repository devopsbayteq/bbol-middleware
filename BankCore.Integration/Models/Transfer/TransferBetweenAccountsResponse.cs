using System.Text.Json.Serialization;

namespace BankCore.Integration.Models.Transfer;

/// <summary>
/// Respuesta de transferencia entre cuentas (BankCore).
/// </summary>
public sealed class TransferBetweenAccountsResponse
{
    [JsonPropertyName("fechaProceso")]
    public string FechaProceso { get; set; } = string.Empty;

    [JsonPropertyName("fechaTransaccion")]
    public string FechaTransaccion { get; set; } = string.Empty;

    [JsonPropertyName("secuenciaTransaccion")]
    public string SecuenciaTransaccion { get; set; } = string.Empty;
}
