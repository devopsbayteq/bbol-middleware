using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Models;
using LogicApi.Model.Response.Security;

namespace LogicApi.Model.Request.Security;
/// <summary>
/// Request for get public key operations
/// </summary>
public class GetPublicKeyRequest : IApiBaseRequest<GetPublicKeyResponse>
{
    /// <inheritdoc />
    [JsonIgnore]
    [IgnoreSensible]
    public ContextRequest ContextRequest { get; set; }
}