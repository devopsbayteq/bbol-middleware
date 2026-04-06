namespace LogicApi.Model.Response.Security;

/// <summary>
/// Response para encriptación RSA.
/// </summary>
public class RsaEncryptTextResponse
{
    /// <summary>
    /// Texto encriptado (Base64).
    /// </summary>
    public string EncryptText { get; set; } = string.Empty;
}
