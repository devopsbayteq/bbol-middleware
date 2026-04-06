using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersistenceDb.Models.Attributes;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Models.Enums;

namespace PersistenceDb.Models.Core;
/// <summary>
/// Tabla Cuentas de Usuario
/// </summary>
[Table(name: "CUENTAS_USUARIO", Schema = "CORE")]
public class AccountUser
{
    /// <summary>
    /// Identificador de cuenta de usuario
    /// </summary>
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("CTU_GUID")]
    public Guid Guid { get; set; }

    /// <summary>
    /// Fecha de registro
    /// </summary>
    [Required]
    [Column("CTU_FECHA_REGISTRO")]
    public DateTime RegisterDate { get; set; }

    /// <summary>
    /// Número de Cuenta
    /// </summary>
    [Required]
    [Column("CTU_NUMERO_CUENTA")]
    [StringLength(128)]
    [EncryptColumn]
    public string AccountNumber { get; set; }

    /// <summary>
    /// Identificador del usuario
    /// </summary>
    [Required]
    [Column("USR_GUID")]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    /// <summary>
    /// Tipo de cuenta.
    /// </summary>
    [Required]
    [Column("CTU_TIPO_CUENTA")]
    public AccountType AccountType { get; set; }

    /// <summary>
    /// Usuario
    /// </summary>
    /// <value></value>
    public User User { get; set; }

}
