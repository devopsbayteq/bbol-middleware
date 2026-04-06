namespace PersistenceDb.Models.Enums;

/// <summary>
/// Tipo de transaccion (filtros y almacenamiento).
/// </summary>
public enum TransactionType : byte
{
    /// <summary>
    /// Transferencias realizadas (salida)
    /// </summary>
    SentTransfers = 1,

    /// <summary>
    /// Pago de servicios
    /// </summary>
    ServicePayments = 2,

    /// <summary>
    /// Otro
    /// </summary>
    Other = 3,

    /// <summary>
    /// Transferencias recibidas (entrada)
    /// </summary>
    ReceivedTransfers = 4,

    /// <summary>
    /// Retiros
    /// </summary>
    Withdrawals = 5,

    /// <summary>
    /// Depósitos
    /// </summary>
    Deposits = 6,

    /// <summary>
    /// Consumos con tarjeta
    /// </summary>
    CardPurchases = 7
}
