namespace PersistenceDb.Models.Enums;

/// <summary>
/// Valores de BEN_BENEFICIARY_TYPE (TINYINT: 1 = propias, 2 = externas por defecto).
/// </summary>
public enum BeneficiaryTypeId : byte
{
    /// <summary>
    /// Cuentas propias del usuario.
    /// </summary>
    OwnAccounts = 1,

    /// <summary>
    /// Cuentas o contactos externos (valor por defecto al insertar).
    /// </summary>
    ExternalAccounts = 2
}
