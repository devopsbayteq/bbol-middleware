using System.Security.Cryptography;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Authentication;
using LogicApi.Model.Response.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.Authentication;

/// <summary>
/// Handler para login biométrico
/// </summary>
public class BiometricLoginHandler(
    ILogger<BiometricLoginHandler> logger,
    IOptions<AppSetting> options,
    IUnitOfWork unitOfWork
    ) : AuthenticationBase<BiometricLoginRequest, LoginResponse>(logger, options)
{
    public async override Task<LoginResponse> Handle(BiometricLoginRequest request, CancellationToken cancellationToken)
    {
        var usernameEncrypted = request.UsernameEncryptBase64.Decode();
        var username = RsaSecurity.Decrypt(AppSettings.RsaSecurity.ServerBase64PrivateKey, usernameEncrypted);
        if (username.IsNullOrEmpty())
            throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo validar el username encriptado.");
        var deviceGuid = (request.ContextRequest?.Headers?.DeviceId) ?? throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo obtener el DeviceGuid del contexto.");
        var device = await unitOfWork.DeviceRepository.GetByFirstOrDefaultAsync(
            where => where.DeviceId == deviceGuid).ConfigureAwait(false) ?? throw new CustomException(MessageCodes.InvalidCredentials, "El dispositivo no está registrado para el usuario.");
        var userEntity = await unitOfWork.UserRepository.GetByFirstOrDefaultAsync(
            where => where.UserName == username).ConfigureAwait(false) ?? throw new CustomException(MessageCodes.InvalidCredentials, "El usuario no existe.");
        EnsureUserNotBlocked(userEntity);
        var expectedChallenge = await unitOfWork.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(
            where => where.UserId == userEntity.Guid && where.DeviceId == device.Guid).ConfigureAwait(false);
        if (expectedChallenge is null || !string.Equals(expectedChallenge.BiometricChallenge, request.Challenge, StringComparison.Ordinal))
            throw new CustomException(MessageCodes.DataDoesNotMatch, "El challenge no coincide con el generado.");

        var mobilePublicKey = device.BiometricPublicKey;
        if (string.IsNullOrWhiteSpace(mobilePublicKey))
            throw new CustomException(MessageCodes.DataDoesNotMatch, "No existe llave pública biométrica registrada para el usuario.");

        var isValidSign = RsaSecurity.VerifySign(
            mobilePublicKey,
            request.Challenge,
            request.ChallengeSignBase64,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        if (!isValidSign)
            throw new CustomException(MessageCodes.DataDoesNotMatch, "La firma biométrica del challenge es inválida.");
        userEntity.FailedLoginAttempts = 0;
        _ = await unitOfWork.UserRepository.UpdateAsync(userEntity).ConfigureAwait(false);
        await EnsureOwnAccountsAsBeneficiariesAsync(userEntity, unitOfWork, cancellationToken).ConfigureAwait(false);
        return await GetLoginResponse(userEntity, device);
    }
}
