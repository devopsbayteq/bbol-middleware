namespace Common.WebApi.Models.AppSettings;

/// <summary>
/// Configuración de RSA (sección <c>RsaSecurity</c> en appsettings o secretos de usuario).
/// </summary>
public sealed class RsaSecuritySettings
{
    /// <summary>
    /// Llave pública del servidor
    /// </summary>
    /// <value></value>
    public string ServerBase64PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Llave pública del dispositivo
    /// </summary>
    /// <value></value>
    public string DeviceBase64PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Llave privada del servidor
    /// </summary>
    /// <value></value>
    public string ServerCertificateBase64PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// Llave pública del servidor
    /// </summary>
    /// <value></value>
    public string ServerCertificateBase64PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Llave privada del servidor
    /// </summary>
    /// <value></value>
    public string ServerBase64PrivateKey { get; set; } = string.Empty;
}
