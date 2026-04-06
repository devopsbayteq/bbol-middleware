using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistenceDb.Models.Core;

[Table(name: "BENEFICIARIO", Schema = "CORE")]
public class Beneficiary
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("BEN_GUID")]
    public Guid Id { get; set; }

    [Required]
    [StringLength(150)]
    [Column("BEN_NOMBRE")]
    public string Name { get; set; }

    [Required]
    [StringLength(20)]
    [Column("BEN_IDENTIFICACION")]
    public string Identification { get; set; }

    [Required]
    [Column("BEN_TIPO_CUENTA")]
    public byte AccountType { get; set; }

    [Required]
    [StringLength(100)]
    [Column("BEN_NOMBRE_BANCO")]
    public string BankName { get; set; }

    /// <summary>
    /// Número de cuenta del beneficiario
    /// </summary>
    [Required]
    [StringLength(128)]
    [Column("BEN_NUMERO_CUENTA")]
    public string AccountNumber { get; set; }
}
