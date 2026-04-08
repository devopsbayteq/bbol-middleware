using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.User;
using LogicApi.Model.Request.Authentication;
using LogicApi.Model.Response.Authentication;
using Common.WebApi.Models.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Common.WebApi.Extensions;
using Common.WebApi.Models;
using Common.WebApi.Security;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using Common.WebApi.Clock;
using BankCore.Integration.Exceptions;

namespace LogicApi.BusinessLogic.Authentication;
/// <summary>
/// Handler for login operations
/// </summary>
public class LoginHandler(
    ILogger<LoginHandler> logger,
    IOptions<AppSetting> options,
    IClock clock,
    IUnitOfWork unitOfWork,
    IBankCoreServices bankCoreServices
        ) : AuthenticationBase<LoginRequest, LoginResponse>(logger, options, bankCoreServices)
{

    /// <summary>
    /// Handle the login request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        using (unitOfWork)
        {
            var decodeUsername = request.Username.Decode();
            var decryptUsername = RsaSecurity.Decrypt(AppSettings.RsaSecurity.ServerBase64PrivateKey, decodeUsername);
            var decodePassword = request.Password.Decode();
            var decryptPassword = RsaSecurity.Decrypt(AppSettings.RsaSecurity.ServerBase64PrivateKey, decodePassword);
            var sessionId = request.ContextRequest?.RequestId ?? Guid.NewGuid().ToString("N");

            var user = await unitOfWork.UserRepository.GetByFirstOrDefaultAsync(
            where => where.UserName == decryptUsername || where.Alias == decryptUsername).ConfigureAwait(false)
            ?? throw new CustomException(MessageCodes.InvalidCredentials, "Usuario no existe.");

            EnsureUserNotBlocked(user);

            if (string.IsNullOrWhiteSpace(AppSettings.LoginSecurity.DummyPassword))
                throw new CustomException(MessageCodes.SystemError, "La configuración de seguridad de login no está definida.");

            if (!string.Equals(decryptPassword, AppSettings.LoginSecurity.DummyPassword, StringComparison.Ordinal))
            {
                await RegisterFailedDummyPasswordAttemptAsync(user).ConfigureAwait(false);

                throw new CustomException(MessageCodes.InvalidCredentials, "Contraseña incorrecta.");
            }

            await ValidateUserAsync(decryptUsername, decryptPassword, sessionId, cancellationToken).ConfigureAwait(false);

            user.FailedLoginAttempts = 0;
            _ = await unitOfWork.UserRepository.UpdateAsync(user).ConfigureAwait(false);

            var deviceGuid = (request.ContextRequest?.Headers?.DeviceId)
                ?? throw new CustomException(MessageCodes.InvalidCredentials, "No se pudo obtener el DeviceId del contexto.");
            var device = await unitOfWork.DeviceRepository.GetByFirstOrDefaultAsync(
                where => where.UserGuid == user.Guid && where.DeviceId == deviceGuid).ConfigureAwait(false);

            device ??= await unitOfWork.DeviceRepository.AddAsync(new Device
            {
                Guid = Guid.NewGuid(),
                UserGuid = user.Guid,
                PlatformType = (byte)PlatformType.Web,
                DeviceId = deviceGuid,
                Model = request.ContextRequest?.Headers?.Model ?? "Unknown",
                Brand = request.ContextRequest?.Headers?.Brand ?? "Unknown",
                RegisterDate = clock.Now(),
            }).ConfigureAwait(false);


            await EnsureOwnAccountsAsBeneficiariesAsync(user, unitOfWork, cancellationToken).ConfigureAwait(false);

            return await GetLoginResponse(user, device);
        }
    }

    private async Task RegisterFailedDummyPasswordAttemptAsync(User user)
    {
        var maxAttempts = Math.Clamp(AppSettings.LoginSecurity.MaxFailedLoginAttempts, 1, 255);
        user.FailedLoginAttempts = user.FailedLoginAttempts < byte.MaxValue
            ? (byte)(user.FailedLoginAttempts + 1)
            : byte.MaxValue;
        if (user.FailedLoginAttempts >= maxAttempts)
            user.LockDate = clock.UtcNow();

        _ = await unitOfWork.UserRepository.UpdateAsync(user).ConfigureAwait(false);
        EnsureUserNotBlocked(user);
    }
}
