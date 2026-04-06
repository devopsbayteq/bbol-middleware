using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PersistenceDb.Models.Authentication;
/// <summary>
/// Tabla Dispositivo
/// </summary>
[Table(name: "DISPOSITIVO", Schema = "AUTHENTICATION")]
public class Device 
{
    /// <summary>
    /// Identificador de dispositivo
    /// </summary>
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("DIS_GUID")]
    public Guid Guid { get; set; }

    /// <summary>
    /// Identificador del usuario
    /// </summary>
    [Required]
    [Column("USR_GUID")]
    [ForeignKey(nameof(User))]
    public Guid UserGuid { get; set; }

    /// <summary>
    /// Plataforma del dispositivo
    /// </summary>
    [Required]
    [Column("DIS_CODIGO_PLATAFORMA")]
    public byte PlatformType { get; set; }

    /// <summary>
    /// Token para notificaciones push
    /// </summary>
    [Column("DIS_GUID_DISPOSITIVO")]
    public string DeviceId { get; set; }

    /// <summary>
    /// Modelo de dispositivo
    /// </summary>
    [Required]
    [Column("DIS_MODELO")]
    public string Model { get; set; }

    /// <summary>
    /// Marca del dispositivo
    /// </summary>
    [Required]
    [Column("DIS_MARCA")]
    public string Brand { get; set; }

    /// <summary>
    /// Fecha de registro
    /// </summary>
    [Required]
    [Column("DIS_FECHA_REGISTRO")]
    public DateTime RegisterDate { get; set; }

    [Column("DIS_BIOMETRICO_PUBLIC_KEY")]
    public string BiometricPublicKey { get; set; }

    /// <summary>
    /// Usuario
    /// </summary>
    /// <value></value>
    public User User { get; set; }
}
