using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Authorization;
using MediatR;

namespace LogicApi.Model.Request.Authorization;

/// <summary>
/// Solicitud para obtener/validar certificado.
/// </summary>
public class GetCertificateRequest : IApiBaseRequest<GetCertificateResponse>
{
    /// <summary>
    /// Base 64 del Secreto generado y encriptado
    /// </summary>
    [IgnoreSensible]
    [Required]
    public string SecretEncryptBase64 { get; set; }

    /// <summary>
    /// Base 64 del resultado de firmar el encriptado del secreto
    /// </summary>
    [Required]
    public string SecretEncryptSignBase64 { get; set; }

    /// <summary>
    /// Base 64 del secreto IV generado y encriptado
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string SecretIvEncryptBase64 { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; } 
}
