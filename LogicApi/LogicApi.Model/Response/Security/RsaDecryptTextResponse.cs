namespace LogicApi.Model.Response.Security;

/// <summary>
/// Response para desencriptación RSA.
/// </summary>
public class RsaDecryptTextResponse
{
    /// <summary>
    /// Texto plano.
    /// </summary>
    public string PlainText { get; set; } = string.Empty;
}
