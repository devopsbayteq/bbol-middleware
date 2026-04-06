using ApiCore.Models;
using Asp.Versioning;
using LogicApi.Model.Request.Authorization;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response;
using LogicApi.Model.Response.Authorization;
using LogicApi.Model.Response.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCore.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SecurityController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Get public key endpoint
    /// </summary>
    /// <param name="request">Get public key request</param>
    /// <returns>Get public key response</returns>
    [HttpGet("public-key")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<GetPublicKeyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicKey([FromQuery] GetPublicKeyRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Obtiene datos de certificado validados (hash encriptado y firma).
    /// </summary>
    /// <param name="request">Datos en Base64 del secreto y firmas</param>
    /// <returns>Certificado validado y bandera de validación de hash</returns>
    [HttpPost("certificate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<GetCertificateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCertificate([FromBody] GetCertificateRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Valida OTP
    /// </summary>
    /// <param name="request">OTP a validar</param>
    /// <returns>Operacion generica</returns>
    [HttpPost("validate-otp")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<GenericCommonOperationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Registra biometrico
    /// </summary>
    /// <param name="request">Certificado biometrico</param>
    /// <returns>Operacion generica</returns>
    [HttpPost("biometric-registration")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<GenericCommonOperationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterBiometric([FromBody] RegisterBiometricRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Valida monto de transferencia
    /// </summary>
    /// <param name="request">Datos de transferencia</param>
    /// <returns>Resultado de validacion</returns>
    [HttpPost("validate-transaction-amount")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse<ValidateTransactionAmountResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateTransactionAmount([FromBody] ValidateTransactionAmountRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Genera challenge aleatorio seguro para biometrico
    /// </summary>
    /// <param name="request">Usuario encriptado</param>
    /// <returns>Challenge aleatorio</returns>
    [HttpPost("biometric-challenge")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<GenerateBiometricChallengeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateBiometricChallenge([FromBody] GenerateBiometricChallengeRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));

    /// <summary>
    /// Encripta texto con AES
    /// </summary>
    /// <param name="request">Texto a encriptar</param>
    /// <returns>Texto encriptado</returns>
    [HttpPost("rsa/encrypt")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse<RsaEncryptTextResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RsaEncrypt([FromBody] RsaEncryptTextRequest request)
        => Success(await Mediator.Send(request).ConfigureAwait(false));
}
