using Microsoft.Extensions.Logging;

namespace LogicApi.BusinessLogic;

public abstract class BusinessLogicBase(ILogger<BusinessLogicBase> logger)
{
    protected readonly ILogger<BusinessLogicBase> Logger = logger;


    /// <summary>
    /// Máscara de cuenta bancaria
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <returns></returns>
    protected static string MaskAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return string.Empty;

        var lastFour = accountNumber.Length >= 4 ? accountNumber[^4..] : accountNumber;
        return $"******{lastFour}";
    }
}
