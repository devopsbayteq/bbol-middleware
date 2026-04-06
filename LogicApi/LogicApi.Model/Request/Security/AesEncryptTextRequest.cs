using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Security;

namespace LogicApi.Model.Request.Security;

/// <summary>
/// Request para encriptar texto con AES.
/// </summary>
public class AesEncryptTextRequest : IApiBaseRequest<AesEncryptTextResponse>
{
    /// <summary>
    /// Texto plano a encriptar.
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string PlainText { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
