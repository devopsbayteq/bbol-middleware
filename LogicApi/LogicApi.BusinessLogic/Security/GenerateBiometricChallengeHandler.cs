using System.Security.Cryptography;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para generar challenge biometrico
/// </summary>
public class GenerateBiometricChallengeHandler(
    ILogger<GenerateBiometricChallengeHandler> logger,
    IOptions<AppSetting> options,
    IUnitOfWork unitOfWork
    ) : SecurityBase<GenerateBiometricChallengeRequest, GenerateBiometricChallengeResponse>(logger)
{
    public async override Task<GenerateBiometricChallengeResponse> Handle(GenerateBiometricChallengeRequest request, CancellationToken cancellationToken)
    {
        var decodeUserEncrypt = request.UserEncryptBase64.Decode();
        var user = RsaSecurity.Decrypt(options.Value.RsaSecurity.ServerBase64PrivateKey, decodeUserEncrypt);
        if (user.IsNullOrEmpty())
            throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo validar el usuario encriptado.");
        var userEntity = await unitOfWork.UserRepository.GetByFirstOrDefaultAsync(
            where => where.UserName == user).ConfigureAwait(false) ?? throw new CustomException(MessageCodes.InvalidCredentials, "El usuario no existe.");

        var deviceGuid = (request.ContextRequest?.Headers?.DeviceId) ?? throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo obtener el DeviceGuid del contexto.");
        var deviceEntity = await unitOfWork.DeviceRepository.GetByFirstOrDefaultAsync(
            where => where.DeviceId == deviceGuid).ConfigureAwait(false) ?? throw new CustomException(MessageCodes.InvalidCredentials, "No existe dispositivo registrado para el usuario.");
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var challenge = Convert.ToBase64String(randomBytes);
        var currentChallenge = (await unitOfWork.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(
            where => where.UserId == userEntity.Guid && where.DeviceId == deviceEntity.Guid).ConfigureAwait(false))
            ?? await unitOfWork.UserDeviceChallengeRepository.AddAsync(new UserDeviceChallenge
            {
                UserId = userEntity.Guid,
                DeviceId = deviceEntity.Guid,
                BiometricChallenge = challenge
            }).ConfigureAwait(false);

        currentChallenge.BiometricChallenge = challenge;
        await unitOfWork.UserDeviceChallengeRepository.UpdateAsync(currentChallenge).ConfigureAwait(false);

        return await Task.FromResult(new GenerateBiometricChallengeResponse
        {
            Challenge = challenge
        });
    }
}
