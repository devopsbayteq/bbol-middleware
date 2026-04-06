using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersistenceDb.Models.Authentication;

namespace PersistenceDb.Models.Core;

[Table(name: "USUARIO_DISPOSITIVO_DESAFIO", Schema = "CORE")]
public class UserDeviceChallenge
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("UDS_GUID")]
    public Guid Id { get; set; }

    [Required]
    [Column("USR_GUID")]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [Required]
    [Column("DIS_GUID")]
    [ForeignKey(nameof(Device))]
    public Guid DeviceId { get; set; }

    [Required]
    [StringLength(64)]
    [Column("UDS_RETO_BIOMETRICO")]
    public string BiometricChallenge { get; set; }

    public User User { get; set; }
    public Device Device { get; set; }
}
