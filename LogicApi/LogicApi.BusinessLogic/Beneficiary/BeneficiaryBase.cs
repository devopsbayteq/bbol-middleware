using MediatR;
using Microsoft.Extensions.Logging;

namespace LogicApi.BusinessLogic.Beneficiary;

/// <summary>
/// Clase base para operaciones de beneficiarios
/// </summary>
public abstract class BeneficiaryBase<TRequest, TResponse>(
    ILogger<BeneficiaryBase<TRequest, TResponse>> logger
    ) : BusinessLogicBase(
        logger
        ),
    IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
