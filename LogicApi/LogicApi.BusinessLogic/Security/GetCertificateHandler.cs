using System.Security.Cryptography;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Authorization;
using LogicApi.Model.Response.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicApi.BusinessLogic.Security;
/// <summary>
/// Constructor
/// </summary>
/// <param name="logger"></param>
/// <param name="pluginFactory"></param>
public class GetCertificateHandler(
    ILogger<GetCertificateHandler> logger,
    IOptions<AppSetting> options)
    : SecurityBase<GetCertificateRequest, GetCertificateResponse>(logger)
{

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<GetCertificateResponse> Handle(GetCertificateRequest request, CancellationToken cancellationToken)
    {
        var secretEncryptDecode = request.SecretEncryptBase64.Decode();
        var secretEncryptSignBase64Decode = request.SecretEncryptSignBase64.Decode();
        var secretIvEncryptBase64Decode = request.SecretIvEncryptBase64.Decode();

        //Obtiene los certificados y valida
        var hashCertificate = options.Value.Certificate.HashCertificate;
        if (hashCertificate.IsNullOrEmpty())
            throw new CustomException(MessageCodes.CertificatesNotFound, "El Hash de Certificado está vacío.");
        //Validamos la Firma
        if (!RsaSecurity.VerifySign(options.Value.RsaSecurity.DeviceBase64PublicKey, secretEncryptDecode, secretEncryptSignBase64Decode, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            throw new CustomException(MessageCodes.DataDoesNotMatch, "No se pudo validar la Firma de la petición.");
        //Desencriptamos con la llave del server y obtenemos el texto plano
        var secretPlainText = RsaSecurity.Decrypt(options.Value.RsaSecurity.ServerCertificateBase64PrivateKey, secretEncryptDecode, RSAEncryptionPadding.OaepSHA1);
        var secretIv = RsaSecurity.Decrypt(options.Value.RsaSecurity.ServerCertificateBase64PrivateKey, secretIvEncryptBase64Decode, RSAEncryptionPadding.OaepSHA1);
        //Creamos un Aes para la verificación y encripción
        var aesEncrypt = AesSecurity.EncryptAes(hashCertificate, secretPlainText, secretIv);
        var rsaEncrypt = RsaSecurity.Encrypt(options.Value.RsaSecurity.DeviceBase64PublicKey, aesEncrypt, RSAEncryptionPadding.OaepSHA1);
        if (Logger.IsEnabled(LogLevel.Debug))
            Logger.LogDebug("Texto Aes: {@AesEncrypt} - Encriptado: {@RsaEncrypt} ", aesEncrypt, rsaEncrypt);
        var rsaSign = RsaSecurity.SignData(options.Value.RsaSecurity.ServerCertificateBase64PrivateKey, rsaEncrypt, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        //Envía la respuesta
        return await Task.FromResult(new GetCertificateResponse(new(rsaEncrypt.Encode(), rsaSign.Encode()), options.Value.Certificate.ValidateHash)).ConfigureAwait(false);
    }
}
