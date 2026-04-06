using Common.WebApi.Attributes.Json;

namespace LogicApi.Model.Response.Security;
/// <summary>
/// Response for get public key operations
/// </summary>
/// <remarks>
/// Get public key response
/// </remarks>
/// <param name="publicKey">Public key</param>
public class GetPublicKeyResponse(string publicKey)
{
    /// <summary>
    /// Public key
    /// </summary>
    [JsonCompresse]
    public string PublicKey { get; set; } = publicKey;
}
