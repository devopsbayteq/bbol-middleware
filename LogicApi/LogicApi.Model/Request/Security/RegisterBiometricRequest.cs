using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response;

namespace LogicApi.Model.Request.Security;

/// <summary>
/// Solicitud para registro biometrico
/// </summary>
public class RegisterBiometricRequest : IApiBaseRequest<GenericCommonOperationResponse>
{
    /// <summary>
    /// Challenge generado previamente por el servidor
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// Firma en Base64 del challenge
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string ChallengeSignBase64 { get; set; } = string.Empty;

    /// <summary>
    /// Llave pública del móvil en Base64
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string MobilePublicKeyBase64 { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
