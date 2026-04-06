using Common.WebApi.Extensions;
using LogicApi.Model.Enums;

namespace LogicApi.Model.Response.Transaction;

/// <summary>
/// Item de transaccion
/// </summary>
public class TransactionItem
{
    /// <summary>
    /// Guid de transaccion
    /// </summary>
    public Guid TransactionGuid { get; set; }

    /// <summary>
    /// Identificador externo de la transacción (ej. referencia de transferencia)
    /// </summary>
    public string TransactionIdentifier { get; set; }

    /// <summary>
    /// Nombre de beneficiario
    /// </summary>
    public string BeneficiaryName { get; set; }

    /// <summary>
    /// Tipo de cuenta del beneficiario (destino)
    /// </summary>
    public AccountType BeneficiaryAccountType { get; set; }

    /// <summary>
    /// Etiqueta del tipo de cuenta del beneficiario
    /// </summary>
    public string BeneficiaryAccountTypeLabel => BeneficiaryAccountType.GetEnumMember();

    /// <summary>
    /// Número de cuenta del beneficiario
    /// </summary>
    public string BeneficiaryAccountNumber { get; set; }

    /// <summary>
    /// Tipo de cuenta de la cuenta origen (titular)
    /// </summary>
    public AccountType OwnerAccountType { get; set; }

    /// <summary>
    /// Etiqueta del tipo de cuenta origen
    /// </summary>
    public string OwnerAccountLabel => OwnerAccountType.GetEnumMember();

    /// <summary>
    /// Número de cuenta origen
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// Tipo de cuenta (destino); mismo valor que <see cref="BeneficiaryAccountType"/> por compatibilidad
    /// </summary>
    public AccountType AccountType { get; set; }

    /// <summary>
    /// Etiqueta tipo cuenta destino (compatibilidad)
    /// </summary>
    public string AccountTypeLabel => AccountType.GetEnumMember();

    /// <summary>
    /// Monto de transaccion
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Fecha de transferencia
    /// </summary>
    public DateTime TransferDate { get; set; }

    /// <summary>
    /// Tipo de transaccion
    /// </summary>
    public string TransactionTypeLabel => TransactionType.GetEnumMember();

    /// <summary>
    /// Tipo de transaccion
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Concepto de la transaccion
    /// </summary>
    public string Concept { get; set; }

    /// <summary>
    /// Saldo acumulado despues de la transaccion
    /// </summary>
    public decimal? BalanceAfterTransaction { get; set; }

    /// <summary>
    /// Indica si la transacción es compartida
    /// </summary>
    public bool AllowedShared => TransactionType == TransactionType.SentTransfers;
}
