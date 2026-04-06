using System.Text.Json.Serialization;

namespace BankCore.Integration.Models.Auth;

/// <summary>
/// Respuesta de token OAuth.
/// </summary>
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
