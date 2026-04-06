namespace LogicApi.Model.Response.Security;

/// <summary>
/// Response para encriptación AES.
/// </summary>
public class AesEncryptTextResponse
{
    /// <summary>
    /// Texto encriptado (Base64).
    /// </summary>
    public string EncryptText { get; set; } = string.Empty;
}
