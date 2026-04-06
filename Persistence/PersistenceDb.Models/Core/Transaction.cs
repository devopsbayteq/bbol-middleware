using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistenceDb.Models.Core;

[Table(name: "TRANSACCION", Schema = "CORE")]
public class Transaction
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("TRX_GUID")]
    public Guid Id { get; set; }

    [Required]
    [Column("TRX_FECHA_REGISTRO")]
    public DateTime RegisterDate { get; set; }

    [Required]
    [Column("CTU_GUID")]
    [ForeignKey(nameof(Account))]
    public Guid AccountGuid { get; set; }

    [Column("BEN_GUID")]
    [ForeignKey(nameof(Beneficiary))]
    public Guid? BeneficiaryGuid { get; set; }

    [Required]
    [Column("TRX_MONTO")]
    public decimal Amount { get; set; }

    [Required]
    [Column("TRX_MONTO_ABSOLUTO")]
    public decimal AbsoluteAmount { get; set; }

    [Required]
    [Column("TRX_MONTO_ANTES_DE_TRANSACCION")]
    public decimal BeforeTransactionAmount { get; set; }

    [Required]
    [Column("TRX_MONTO_RESULTANTE")]
    public decimal ResultingAmount { get; set; }

    [Required]
    [StringLength(300)]
    [Column("TRX_DESCRIPCION")]
    public string Description { get; set; }

    [Required]
    [StringLength(300)]
    [Column("TRX_DESCRIPCION_NORMALIZADA")]
    public string NormalizedDescription { get; set; }

    [StringLength(100)]
    [Column("TRX_IDENTIFICADOR_EXTERNO")]
    public string ExternalIdentifier { get; set; }

    [Required]
    [Column("TRX_TIPO_ID")]
    public byte TransactionType { get; set; }

    public AccountUser Account { get; set; }
    public Beneficiary Beneficiary { get; set; }
}
