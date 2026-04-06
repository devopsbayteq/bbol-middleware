using MediatR;
using Microsoft.Extensions.Logging;

namespace LogicApi.BusinessLogic.ContractBalance;

/// <summary>
/// Clase base para operaciones de saldo de contrato
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class ContractBalanceBase<TRequest, TResponse>(
    ILogger<ContractBalanceBase<TRequest, TResponse>> logger
    ) : BusinessLogicBase(
        logger
        ),
    IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
