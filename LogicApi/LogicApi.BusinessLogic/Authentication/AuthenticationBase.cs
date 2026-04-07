using MediatR;
using Common.WebApi.Models.AppSettings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using LogicApi.Model.Response.Authentication;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using Common.WebApi.Models;
using CoreBeneficiary = PersistenceDb.Models.Core.Beneficiary;
using Common.WebApi.Security;
using Common.WebApi.Extensions;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;

namespace LogicApi.BusinessLogic.Authentication;
/// <summary>
/// Base class for authentication operations
/// </summary>
public abstract class AuthenticationBase<TRequest, TResponse>(
    ILogger<AuthenticationBase<TRequest, TResponse>> logger,
    IOptions<AppSetting> options
    ) : BusinessLogicBase(
        logger
        ),
    IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly AppSetting AppSettings = options.Value;

    /// <summary>
    /// Handle the request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Indica si el usuario tiene cuenta bloqueada por intentos fallidos.
    /// </summary>
    protected static bool IsUserBlocked(User user) =>
        user is not null && user.LockDate.HasValue;

    /// <summary>
    /// Lanza si el usuario está bloqueado (login normal o biométrico).
    /// </summary>
    protected static void EnsureUserNotBlocked(User user)
    {
        if (IsUserBlocked(user))
            throw new CustomException(MessageCodes.AccountLocked, "Su cuenta se encuentra bloqueada en la fecha: " + user.LockDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    /// <summary>
    /// Genera JWT para autenticación
    /// </summary>
    /// <param name="extraClaims"></param>
    /// <returns></returns>
    protected string GenerateAccessToken(IEnumerable<Claim> extraClaims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (extraClaims != null)
            claims.AddRange(extraClaims);

        var token = new JwtSecurityToken(
            AppSettings.Jwt.Issuer,
            AppSettings.Jwt.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(AppSettings.Jwt.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Registra en BENEFICIARIO las cuentas del usuario como tipo propio si aún no existen (mismo número de cuenta y usuario).
    /// </summary>
    protected async Task EnsureOwnAccountsAsBeneficiariesAsync(
        User user,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        const string bankName = "Banco Bolivariano";

        var displayName = $"{user.FirstName} {user.Surname}".Trim();
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = user.FirstName ?? user.Surname ?? "Titular";

        var accounts = await unitOfWork.AccountUserRepository
            .GetByAsync(a => a.UserId == user.Guid)
            .ConfigureAwait(false);
        if (accounts.Count == 0)
            return;

        var existingOwnBeneficiaries = await unitOfWork.BeneficiaryRepository
            .GetByAsync(b => b.UserId == user.Guid && b.BeneficiaryType == BeneficiaryTypeId.OwnAccounts)
            .ConfigureAwait(false);

        var coveredAccountNumbers = existingOwnBeneficiaries
            .Select(b => b.AccountNumber ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);

        var toInsert = new List<CoreBeneficiary>();
        foreach (var account in accounts)
        {
            var accountNumber = account.AccountNumber ?? string.Empty;
            if (coveredAccountNumbers.Contains(accountNumber))
                continue;

            coveredAccountNumbers.Add(accountNumber);
            toInsert.Add(new CoreBeneficiary
            {
                Id = Guid.NewGuid(),
                UserId = user.Guid,
                BeneficiaryType = BeneficiaryTypeId.OwnAccounts,
                Name = displayName,
                Identification = user.DocumentNumber ?? string.Empty,
                AccountType = (byte)account.AccountType,
                BankName = bankName,
                AccountNumber = accountNumber
            });
        }

        if (toInsert.Count > 0)
            _ = await unitOfWork.BeneficiaryRepository.AddRangeAsync(toInsert).ConfigureAwait(false);
    }

    /// <summary>
    /// Obtiene la respuesta de login
    /// </summary>
    /// <param name="user"></param>
    /// <param name="device"></param>
    /// <returns></returns>
    public async Task<LoginResponse> GetLoginResponse(
        User user,
        Device device
    )
    {

        var listClaims = new List<Claim>
           {
            new(nameof(EncryptedFieldClaim), AesSecurity.EncryptAes(new EncryptedFieldClaim{
                UserGuid = user.Guid,
                UserName = user.UserName,
                DeviceId = device.Guid
            }.ToJson(), AppSettings.AesSecurity.Key))
           };
        return await Task.FromResult(new LoginResponse
        {
            AccessToken = GenerateAccessToken(listClaims),
            FirstName = user.FirstName,
            SessionTimeSeconds = AppSettings.Session.SessionTimeSeconds,
            InactivityTimeoutSeconds = AppSettings.Session.InactivityTimeoutSeconds,
            Alias = user.Alias
        });
    }
}