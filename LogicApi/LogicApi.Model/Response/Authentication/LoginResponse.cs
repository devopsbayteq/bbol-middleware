using Common.WebApi.Attributes.Json;

namespace LogicApi.Model.Response.Authentication;
/// <summary>
/// Handler for login operations
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token for the login
    /// </summary>
    /// <value></value>
    [JsonCompresse]
    public string AccessToken { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    /// <value></value>
    public string FirstName { get; set; }

    /// <summary>
    /// Session time in seconds
    /// </summary>
    /// <value></value>
    public int SessionTimeSeconds { get; set; }

    /// <summary>
    /// Inactivity timeout
    /// </summary>
    /// <value></value>
    public int InactivityTimeoutSeconds { get; set; }
}