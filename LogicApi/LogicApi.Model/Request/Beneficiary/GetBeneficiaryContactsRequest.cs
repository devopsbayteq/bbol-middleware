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
}
