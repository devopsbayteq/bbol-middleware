using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Authentication;

namespace LogicApi.Model.Request.Authentication;

/// <summary>
/// Request para login biométrico
/// </summary>
public class BiometricLoginRequest : IApiBaseRequest<LoginResponse>
{
    /// <summary>
    /// Username encriptado en Base64
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string UsernameEncryptBase64 { get; set; } = string.Empty;

    /// <summary>
    /// Challenge recibido
    /// </summary>
    [Required]
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// Firma del challenge en Base64
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string ChallengeSignBase64 { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
