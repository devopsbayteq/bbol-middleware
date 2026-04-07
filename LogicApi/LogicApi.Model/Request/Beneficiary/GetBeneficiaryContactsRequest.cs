using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Beneficiary;

namespace LogicApi.Model.Request.Beneficiary;

/// <summary>
/// Solicitud para obtener contactos beneficiarios
/// </summary>
public class GetBeneficiaryContactsRequest : IApiBaseRequest<GetBeneficiaryContactsResponse>
{
    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }

    /// <summary>
    /// Tipo de beneficiario
    /// </summary>
    public BeneficiaryType BeneficiaryType { get; set; } = BeneficiaryType.ExternalAccounts;
}

public enum BeneficiaryType
{
    /// <summary>
    /// Cuentas propias del usuario.
    /// </summary>
    OwnAccounts = 1,

    /// <summary>
    /// Cuentas o contactos externos (valor por defecto al insertar).
    /// </summary>
    ExternalAccounts = 2
}