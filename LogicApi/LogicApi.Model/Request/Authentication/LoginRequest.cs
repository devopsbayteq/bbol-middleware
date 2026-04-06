using System.Text.Json.Serialization;
using Common.WebApi.Attributes;
using Common.WebApi.Attributes.Json;
using Common.WebApi.Models;
using LogicApi.Model.Response.Authentication;

namespace LogicApi.Model.Request.Authentication;
/// <summary>
/// Request for login operations
/// </summary>
public class LoginRequest : IApiBaseRequest<LoginResponse>
{
    /// <summary>
    /// Username
    /// </summary>
    /// <value></value>
    [JsonCompresse]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    /// <value></value>
    [JsonCompresse]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Context
    /// </summary>
    [JsonIgnore]
    [IgnoreSensible]

    public ContextRequest ContextRequest { get; set; } 
}