using Common.WebApi.Extensions;
using LogicApi.Model.Enums;

namespace LogicApi.Model.Response.Beneficiary;

/// <summary>
/// Respuesta de contactos beneficiarios
/// </summary>
public class GetBeneficiaryContactsResponse
{
    /// <summary>
    /// Contactos
    /// </summary>
    public List<BeneficiaryContactItem> Contacts { get; set; } = [];
}

/// <summary>
/// Contacto beneficiario
/// </summary>
public class BeneficiaryContactItem
{
    /// <summary>
    /// Guid de beneficiario
    /// </summary>
    public Guid BeneficiaryGuid { get; set; } = Guid.Empty;

    /// <summary>
    /// Nombre de contacto
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del banco
    /// </summary>
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de cuenta del beneficiario
    /// </summary>
    public AccountType AccountType { get; set; }

    /// <summary>
    /// Identificacion del beneficiario
    /// </summary>
    public string AccountTypeLabel  => AccountType.GetEnumMember();

    /// <summary>
    /// Número de cuenta del beneficiario
    /// </summary>
    public string BeneficiaryAccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Ultimos 4 digitos de cuenta
    /// </summary>
    public string LastFourDigits { get; set; } = string.Empty;
}
