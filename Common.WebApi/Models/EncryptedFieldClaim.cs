namespace Common.WebApi.Models;
/// <summary>
/// Datos encritados en claim
/// </summary>
public class EncryptedFieldClaim
{
    /// <summary>
    /// Id de usuario 
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// Id de Dispositivo
    /// </summary>
    /// <value></value>
    public Guid? DeviceId { get; set; }

    /// <summary>
    /// Nombre de Usuario
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Número de Identificación
    /// </summary>
    public string IdentificationNumber { get; set; }
}