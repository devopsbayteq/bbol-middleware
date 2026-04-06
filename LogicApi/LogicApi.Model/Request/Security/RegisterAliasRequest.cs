using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response;

namespace LogicApi.Model.Request.Security;

/// <summary>
/// Solicitud para registrar el alias de cuenta del usuario autenticado.
/// </summary>
public class RegisterAliasRequest : IApiBaseRequest<GenericCommonOperationResponse>
{
    /// <summary>
    /// Alias encriptado con la llave pública del servidor (mismo esquema que el usuario en login).
    /// </summary>
    [Required]
    [IgnoreSensible]
    public string Alias { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
