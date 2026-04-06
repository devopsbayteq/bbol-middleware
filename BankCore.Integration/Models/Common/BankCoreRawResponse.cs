using System.Text.Json;

namespace BankCore.Integration.Models.Common;

/// <summary>
/// Wrapper de respuesta tolerante para APIs externas.
/// </summary>
public class BankCoreRawResponse
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string RawContent { get; set; } = string.Empty;
    public JsonElement? Data { get; set; }
}
