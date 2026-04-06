using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Security;

namespace LogicApi.Model.Request.Security;

/// <summary>
/// Request para desencriptar texto con AES.
/// </summary>
public class AesDecryptTextRequest : IApiBaseRequest<AesDecryptTextResponse>
{
    /// <summary>
    /// Texto encriptado (Base64) a desencriptar.
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string EncryptText { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
