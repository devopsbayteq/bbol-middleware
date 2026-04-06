using MediatR;
using Microsoft.Extensions.Logging;

namespace LogicApi.BusinessLogic.Security;
/// <summary>
/// Clase base de seguridades
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class SecurityBase<TRequest, TResponse>(
    ILogger<SecurityBase<TRequest, TResponse>> logger
    ) : BusinessLogicBase(
        logger
        ),
    IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
