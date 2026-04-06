namespace PersistenceDb.Models.Enums;

/// <summary>
/// Tipo de cuenta del usuario.
/// </summary>
public enum AccountType : byte
{
    /// <summary>
    /// Cuenta de débito.
    /// </summary>
    Savings = 1,

    /// <summary>
    /// Cuenta de crédito.
    /// </summary>
    Checking = 2
}
