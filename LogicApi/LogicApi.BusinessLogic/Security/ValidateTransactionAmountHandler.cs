using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para validar monto de transaccion
/// </summary>
public class ValidateTransactionAmountHandler(
    ILogger<ValidateTransactionAmountHandler> logger
    ) : SecurityBase<ValidateTransactionAmountRequest, ValidateTransactionAmountResponse>(logger)
{
    public override Task<ValidateTransactionAmountResponse> Handle(ValidateTransactionAmountRequest request, CancellationToken cancellationToken)
    {
        var isValid = request.Amount > 20m;

        return Task.FromResult(new ValidateTransactionAmountResponse
        {
            IsValid = isValid
        });
    }
}
