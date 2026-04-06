using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersistenceDb.Models.Attributes;
using PersistenceDb.Models.Enums;

namespace PersistenceDb.Models.Authentication;
/// <summary>
/// Tabla Usuario
/// </summary>
[Table(name: "USUARIO", Schema = "AUTHENTICATION")]
public class User
{
    /// <summary>
    /// Id
    /// </summary>
    /// <value></value>
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("USR_GUID")]
    public Guid Guid { get; set; }

    /// <summary>
    /// Fecha de registro.
    /// </summary>
    [Column("USR_FECHA_REGISTRO")]
    public DateTime RegisterDate { get; set; }

    /// <summary>
    /// Nombre de Usuario
    /// </summary>
    /// <value></value>
    [Required]
    [Column("USR_USERNAME")]
    [EncryptColumn]
    public string UserName { get; set; }

    /// <summary>
    /// Tipo de Documento
    /// </summary>
    /// <value></value>
    [Required]
    [Column("USR_TIPO_DOCUMENTO")]
    public IdentificationType IdentificationType { get; set; }

    /// <summary>
    /// Documento
    /// </summary>
    /// <value></value>
    [Required]
    [Column("USR_NUMERO_DOCUMENTO")]
    [EncryptColumn]
    public string DocumentNumber { get; set; }

    /// <summary>
    /// Primer Nombre (Extraído del Name)
    /// </summary>
    [Required]
    [Column("USR_PRIMER_NOMBRE")]
    public string FirstName { get; set; }

    /// <summary>
    /// Apellido (Extraído del Name)
    /// </summary>
    [Required]
    [Column("USR_PRIMER_APELLIDO")]
    public string Surname { get; set; }


    [Required]
    [Column("USR_ALIAS")]
    [EncryptColumn]
    public string Alias { get; set; }

    /// <summary>
    /// Fecha de 1° login.
    /// </summary>
    [Column("USR_FECHA_PRIMER_LOGIN")]
    public DateTime FirstLoginDate { get; set; }

    /// <summary>
    /// Fecha en que la cuenta quedó bloqueada por intentos fallidos (null = no bloqueada).
    /// </summary>
    [Column("USR_FECHA_BLOQUEO")]
    public DateTime? LockDate { get; set; }

    /// <summary>
    /// Intentos fallidos de login con contraseña dummy consecutivos.
    /// </summary>
    [Required]
    [Column("USR_INTENTOS_FALLIDOS")]
    public byte FailedLoginAttempts { get; set; }

    /// <summary>
    /// Relación con los dispositivos
    /// </summary>
    public List<Device> Devices { get; set; }

}