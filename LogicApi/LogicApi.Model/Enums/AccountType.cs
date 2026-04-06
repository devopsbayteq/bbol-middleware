using System.Runtime.Serialization;

namespace LogicApi.Model.Enums;

/// <summary>
/// Tipo de cuenta
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Cuenta de ahorros
    /// </summary>
    [EnumMember(Value = "Cta. Ahorros")]
    Savings = 1,

    /// <summary>
    /// Cuenta corriente
    /// </summary>
    [EnumMember(Value = "Cta. Corriente")]
    Checking = 2
}
