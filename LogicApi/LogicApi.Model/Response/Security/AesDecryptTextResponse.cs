namespace LogicApi.Model.Response.Security;

/// <summary>
/// Response para desencriptación AES.
/// </summary>
public class AesDecryptTextResponse
{
    /// <summary>
    /// Texto plano.
    /// </summary>
    public string PlainText { get; set; } = string.Empty;
}
