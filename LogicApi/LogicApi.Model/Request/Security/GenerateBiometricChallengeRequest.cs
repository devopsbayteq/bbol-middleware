using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Security;

namespace LogicApi.Model.Request.Security;

/// <summary>
/// Solicitud para generar challenge biometrico
/// </summary>
public class GenerateBiometricChallengeRequest : IApiBaseRequest<GenerateBiometricChallengeResponse>
{
    /// <summary>
    /// Usuario encriptado y codificado en Base64
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string UserEncryptBase64 { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
