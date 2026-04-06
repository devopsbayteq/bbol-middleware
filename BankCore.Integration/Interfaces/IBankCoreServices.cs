using BankCore.Integration.Models.Common;
using BankCore.Integration.Models.Transfer;
using BankCore.Integration.Models.User;

namespace BankCore.Integration.Interfaces;

/// <summary>
/// Contrato de servicios core del banco.
/// </summary>
public interface IBankCoreServices
{
    /// <summary>
    /// Obtiene token OAuth reutilizable para consumos posteriores.
    /// </summary>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>   
    /// Valida usuario.
    /// </summary>
    /// <param name="request">Solicitud de validación de usuario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Respuesta de la validación de usuario.</returns>
    Task<ValidateUserResponse> ValidateUserAsync(ValidateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida clave de usuario.
    /// </summary>
    /// <param name="request">Solicitud de validación de clave de usuario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Respuesta de la validación de clave de usuario.</returns>
    Task<ValidateUserPasswordResponse> ValidateUserPasswordAsync(ValidateUserPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Realiza transferencia entre cuentas.
    /// </summary>
    /// <param name="request">Solicitud de transferencia entre cuentas.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Respuesta de la transferencia entre cuentas.</returns>
    Task<TransferBetweenAccountsResponse> TransferBetweenAccountsAsync(TransferBetweenAccountsRequest request, CancellationToken cancellationToken = default);
}
