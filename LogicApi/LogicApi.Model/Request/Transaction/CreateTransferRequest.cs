using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Transaction;

namespace LogicApi.Model.Request.Transaction;

/// <summary>
/// Solicitud para crear transferencia
/// </summary>
public class CreateTransferRequest : IApiBaseRequest<CreateTransferResponse>
{
    /// <summary>
    /// Monto de transferencia
    /// </summary>
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Guid de beneficiario o contacto
    /// </summary>
    [Required]
    public Guid BeneficiaryContactGuid { get; set; }

    /// <summary>
    /// Guid de cuenta origen
    /// </summary>
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// Concepto de transferencia
    /// </summary>
    public string Concept { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}
