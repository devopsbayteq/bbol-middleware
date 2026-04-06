using System.Runtime.Serialization;

namespace LogicApi.Model.Enums;

/// <summary>
/// Tipo de transacción (valores alineados con <see cref="PersistenceDb.Models.Enums.TransactionType"/>).
/// </summary>
public enum TransactionType : byte
{
    /// <summary>
    /// Transferencias realizadas
    /// </summary>
    [EnumMember(Value = "Transferencias realizadas")]
    SentTransfers = 1,

    /// <summary>
    /// Pago de servicios
    /// </summary>
    [EnumMember(Value = "Pago de servicios")]
    ServicePayments = 2,

    /// <summary>
    /// Otro
    /// </summary>
    [EnumMember(Value = "Otro")]
    Other = 3,

    /// <summary>
    /// Transferencias recibidas
    /// </summary>
    [EnumMember(Value = "Transferencias recibidas")]
    ReceivedTransfers = 4,

    /// <summary>
    /// Retiros
    /// </summary>
    [EnumMember(Value = "Retiros")]
    Withdrawals = 5,

    /// <summary>
    /// Depósitos
    /// </summary>
    [EnumMember(Value = "Depósitos")]
    Deposits = 6,

    /// <summary>
    /// Consumos con tarjeta
    /// </summary>
    [EnumMember(Value = "Consumos con tarjeta")]
    CardPurchases = 7
}
