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
    /// Alias of the user
    /// </summary>
    /// <value></value>
    public string Alias { get; set; }

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

    /// <summary>
    /// Recent activities
    /// </summary>
    /// <value></value>
    public List<RecentActivity> RecentActivities { get; set; } = [];

}

public class RecentActivity
{
    /// <summary>
    /// Date and time of the activity
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Activity description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Activity type
    /// </summary>
    public decimal Amount { get; set; }
}