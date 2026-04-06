using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Attributes.Json;
using Common.WebApi.Models;
using LogicApi.Model.Response;

namespace LogicApi.Model.Request.Security;

/// <summary>
/// Solicitud para validar OTP
/// </summary>
public class ValidateOtpRequest : IApiBaseRequest<GenericCommonOperationResponse>
{
    /// <summary>
    /// Código OTP
    /// </summary>
    [Required]
    [JsonCompresse]
    public string Otp { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
