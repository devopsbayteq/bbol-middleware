namespace Common.WebApi.Models;

/// <summary>
/// Contexto de auditoría del request
/// </summary>
public class ContextRequest
{

    /// <summary>
    /// Request Id
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Headers de auditoría
    /// </summary>
    public Header Headers { get; set; }


    /// <summary>
    /// Custom Claims
    /// </summary>
    public CustomClaims CustomClaims { get; set; }

}

/// <summary>
/// Custom Claims
/// </summary>
public class CustomClaims
{

    /// <summary>
    /// Id de usuario 
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Nombre de Usuario
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Id de Dispositivo
    /// </summary>
    /// <value></value>
    public Guid? DeviceGuid { get; set; }
}

/// <summary>
/// Headers
/// </summary>
public class Header
{
    /// <summary>
    /// Plataforma declarada en el request
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// Identificador del dispositivo
    /// </summary>
    public string DeviceId { get; set; }

    /// <summary>
    /// Versión de la aplicación
    /// </summary>
    public string VersionApplication { get; set; }

    /// <summary>
    /// Versión de la aplicación
    /// </summary>
    public string VersionSystemOperation { get; set; }

    /// <summary>
    /// Modelo de dispositivo
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Marca del dispositivo
    /// </summary>
    public string Brand { get; set; }

    /// <summary>
    /// Fecha del cliente
    /// </summary>
    public DateTime? ClientDate { get; set; }

    /// <summary>
    /// Unix Time
    /// </summary>
    /// <value></value>
    public long UnixTime { get; set; }

    /// <summary>
    /// Zona Horaria
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// Contenido para validación
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Secreto
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    /// TimeSpan
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// Token de Autorización
    /// </summary>
    /// <value></value>
    public string Authorization { get; set; }
}