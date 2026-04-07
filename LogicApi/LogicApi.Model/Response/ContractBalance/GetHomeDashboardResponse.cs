using Common.WebApi.Attributes;
using Common.WebApi.Extensions;
using LogicApi.Model.Enums;
using LogicApi.Model.Response.Beneficiary;

namespace LogicApi.Model.Response.ContractBalance;

/// <summary>
/// Respuesta del home con productos del cliente
/// </summary>
public class GetHomeDashboardResponse
{
    /// <summary>
    /// Balance total de cuentas del usuario
    /// </summary>
    public decimal TotalBalance { get; set; }

    /// <summary>
    /// Cuentas
    /// </summary>
    public List<HomeAccountItem> Accounts { get; set; } = [];

    /// <summary>
    /// Tarjetas de credito
    /// </summary>
    public List<HomeCreditCardItem> CreditCards { get; set; } = [];

    /// <summary>
    /// Prestamos
    /// </summary>
    public List<HomeLoanItem> Loans { get; set; } = [];

    /// <summary>
    /// Inversiones
    /// </summary>
    public List<HomeInvestmentItem> Investments { get; set; } = [];

    /// <summary>
    /// Pagos frecuentes
    /// </summary>
    public List<HomeFrequentPaymentItem> FrequentPayments { get; set; } = [];
}

/// <summary>
/// Item de cuenta para home
/// </summary>
public class HomeAccountItem
{
    [IgnoreSensible]
    public Guid AccountGuid { get; set; }
    public string MaskedAccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string AccountTypeLabel => AccountType.GetEnumMember();
    public decimal Balance { get; set; }
    
    /// <summary>
    /// Guid del beneficiario
    /// </summary>
    /// <value></value>
    public BeneficiaryContactItem Beneficiary { get; set; }

}

/// <summary>
/// Item de tarjeta de credito para home
/// </summary>
public class HomeCreditCardItem
{
    public string MaskedCardNumber { get; set; } = string.Empty;
    public decimal TotalDue { get; set; }
    public DateTime MaxPaymentDate { get; set; }
}

/// <summary>
/// Item de prestamo para home
/// </summary>
public class HomeLoanItem
{
    public string LoanGuid { get; set; } = string.Empty;
    public decimal OutstandingBalance { get; set; }
    public decimal NextInstallmentAmount { get; set; }
    public DateTime NextInstallmentDate { get; set; }
}

/// <summary>
/// Item de inversion para home
/// </summary>
public class HomeInvestmentItem
{
    public string InvestmentGuid { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Item de pago frecuente para home
/// </summary>
public class HomeFrequentPaymentItem
{
    public string BeneficiaryName { get; set; } = string.Empty;
    public string BeneficiaryType { get; set; } = string.Empty;
}
