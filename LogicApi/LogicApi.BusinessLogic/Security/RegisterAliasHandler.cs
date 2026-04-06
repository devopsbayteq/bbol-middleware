using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Registra el alias de cuenta para el usuario del contexto JWT.
/// </summary>
public class RegisterAliasHandler(
    ILogger<RegisterAliasHandler> logger,
    IOptions<AppSetting> options,
    IUnitOfWork unitOfWork
    ) : SecurityBase<RegisterAliasRequest, GenericCommonOperationResponse>(logger)
{
    public override async Task<GenericCommonOperationResponse> Handle(RegisterAliasRequest request, CancellationToken cancellationToken)
    {
        var userId = request.ContextRequest?.CustomClaims?.UserId
            ?? throw new CustomException(MessageCodes.UserContextNotFound, "No se pudo obtener el UserId del contexto.");

        var decodeAlias = request.Alias.Decode();
        var decryptAlias = RsaSecurity.Decrypt(options.Value.RsaSecurity.ServerBase64PrivateKey, decodeAlias).Trim();
        if (string.IsNullOrWhiteSpace(decryptAlias))
            throw new CustomException(MessageCodes.DataDoesNotMatch, "El alias no es válido.");

        var user = await unitOfWork.UserRepository.GetByFirstOrDefaultAsync(where => where.Guid == userId).ConfigureAwait(false)
            ?? throw new CustomException(MessageCodes.UserContextNotFound, "Usuario no encontrado.");

        if (string.Equals(user.Alias, decryptAlias, StringComparison.Ordinal))
            return GenericCommonOperationResponse.SuccessOperation();

        var conflict = await unitOfWork.UserRepository.GetByFirstOrDefaultAsync(
            where => where.Guid != user.Guid && (where.UserName == decryptAlias || where.Alias == decryptAlias)).ConfigureAwait(false);
        if (conflict is not null)
            throw new CustomException(MessageCodes.AliasAlreadyInUse, "El alias ya está en uso.");

        user.Alias = decryptAlias;
        _ = await unitOfWork.UserRepository.UpdateAsync(user).ConfigureAwait(false);

        return GenericCommonOperationResponse.SuccessOperation();
    }
}
