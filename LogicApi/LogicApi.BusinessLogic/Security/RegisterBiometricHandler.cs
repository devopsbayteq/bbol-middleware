using System.Security.Cryptography;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Security;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response;
using Microsoft.Extensions.Logging;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para registro biometrico
/// </summary>
public class RegisterBiometricHandler(
    ILogger<RegisterBiometricHandler> logger,
    IUnitOfWork unitOfWork
    ) : SecurityBase<RegisterBiometricRequest, GenericCommonOperationResponse>(logger)
{
    public async override Task<GenericCommonOperationResponse> Handle(RegisterBiometricRequest request, CancellationToken cancellationToken)
    {
        var userId = request.ContextRequest?.CustomClaims?.UserId;
        if (!userId.HasValue)
            throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo obtener el UserId del contexto.");
        var deviceGuid = (request.ContextRequest?.Headers?.DeviceId) ?? throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo obtener el DeviceGuid del contexto.");
        var device = await unitOfWork.DeviceRepository.GetByFirstOrDefaultAsync(
            where => where.DeviceId == deviceGuid).ConfigureAwait(false) ?? throw new CustomException(MessageCodes.InvalidCredentials, "No existe dispositivo registrado para el usuario.");
        var expectedChallenge = await unitOfWork.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(
            where => where.UserId == userId.Value && where.DeviceId == device.Guid).ConfigureAwait(false) ?? throw new CustomException(MessageCodes.InvalidCredentials, "No existe challenge registrado para el usuario.");
        if (!string.Equals(expectedChallenge.BiometricChallenge, request.Challenge, StringComparison.Ordinal))
            throw new CustomException(MessageCodes.DataDoesNotMatch, "El challenge no corresponde al generado para el usuario.");

        var isValidSign = RsaSecurity.VerifySign(
            request.MobilePublicKeyBase64,
            request.Challenge,
            request.ChallengeSignBase64,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        if (!isValidSign)
            throw new CustomException(MessageCodes.DataDoesNotMatch, "No se pudo validar la firma del challenge biométrico.");

        device.BiometricPublicKey = request.MobilePublicKeyBase64;
        await unitOfWork.DeviceRepository.UpdateAsync(device).ConfigureAwait(false);
        return await Task.FromResult(GenericCommonOperationResponse.SuccessOperation());
    }
}
