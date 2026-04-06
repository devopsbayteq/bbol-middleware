namespace BankCore.Integration.Models;

/// <summary>
/// Configuración de integración BankCore.
/// </summary>
public class BankCoreOptions
{
    public const string SectionName = "BankCore";

    public bool UseMock { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public int TokenRefreshBeforeExpirySeconds { get; set; } = 30;
    public int TokenRequestTimeoutSeconds { get; set; } = 15;
}
