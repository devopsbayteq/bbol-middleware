namespace Common.WebApi.Models.AppSettings;

/// <summary>
/// Configuración de JWT (sección <c>Jwt</c> en appsettings o secretos de usuario).
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Clave simétrica (UTF-8); mínimo 32 caracteres para HS256.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60;
}
