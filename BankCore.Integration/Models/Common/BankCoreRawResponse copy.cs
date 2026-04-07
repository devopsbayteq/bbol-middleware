using System.Text.Json;
using Newtonsoft.Json;

namespace BankCore.Integration.Models.Common;

/// <summary>
/// Wrapper de respuesta tolerante para APIs externas.
/// </summary>
public class BankCoreErrorResponse
{
    [JsonProperty("codigo")]
    public string Code { get; set; }
    [JsonProperty("mensajeUsuario")]
    public string UserMessage { get; set; }
    [JsonProperty("mensajeSistema")]
    public string SystemMessage { get; set; }
}
