using System.Security.Cryptography;
using System.Text;

namespace Common.WebApi.Security;
/// <summary>
/// RSA Security
/// </summary>
public static class RsaSecurity
{
    /// <summary>
    /// Encripta
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string Encrypt(string publicKeyBase64, string plainText, RSAEncryptionPadding rSAEncryptionPadding = null)
    {
        try
        {
            rSAEncryptionPadding ??= RSAEncryptionPadding.OaepSHA1;
            if (string.IsNullOrEmpty(plainText))
                throw new InvalidOperationException("No hay texto para encriptar");
            var dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            using var rsa = new RSACryptoServiceProvider(2048);
            if (string.IsNullOrEmpty(publicKeyBase64))
                throw new InvalidOperationException("La llave Pública es requerida");
            var key = DecodeKey(publicKeyBase64);
            rsa.ImportFromPem(key.ToCharArray());
            var dataEncripted = rsa.Encrypt(dataToEncrypt, rSAEncryptionPadding);
            return Convert.ToBase64String(dataEncripted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error mientras se intentaba encriptar texto: {plainText}", ex);
        }
    }

    /// <summary>
    /// Desencripta
    /// </summary>
    /// <param name="encryptText"></param>
    /// <returns></returns>
    public static string Decrypt(string privateKeyBase64, string encryptText, RSAEncryptionPadding rSAEncryptionPadding = null)
    {
        try
        {
            rSAEncryptionPadding ??= RSAEncryptionPadding.OaepSHA1;
            if (string.IsNullOrEmpty(encryptText))
                throw new InvalidOperationException("No hay texto para des-encriptar");
            var dataToDecrypt = Convert.FromBase64String(encryptText);
            if (string.IsNullOrEmpty(privateKeyBase64))
                throw new InvalidOperationException("La llave Privada es requerida");
            var key = DecodeKey(privateKeyBase64);
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportFromPem(key.ToCharArray());
            var dataDecrypted = rsa.Decrypt(dataToDecrypt, rSAEncryptionPadding);
            return Encoding.UTF8.GetString(dataDecrypted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error mientras se intentaba desencriptar texto: {encryptText}", ex);
        }
    }


    /// <summary>
    /// Verifica la firma de un texto
    /// <param name="publicKeyBase64">Llave pública</param>
    /// <param name="encryptText">Texto encriptado</param>
    /// <param name="signText">Firma del texto</param>
    /// <param name="hashAlgorithmName">Algoritmo de hash</param>
    /// <param name="rSAEncryptionPadding">Padding de RSA</param>
    /// </summary>
    public static bool VerifySign(string publicKeyBase64, string encryptText, string signText, HashAlgorithmName hashAlgorithmName, RSASignaturePadding rSAEncryptionPadding)
    {
        try
        {
            if (string.IsNullOrEmpty(publicKeyBase64))
                throw new InvalidOperationException("La llave Pública es requerida");
            var key = DecodeKey(publicKeyBase64);
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportFromPem(key.ToCharArray());
            return rsa.VerifyData(Encoding.UTF8.GetBytes(encryptText), Convert.FromBase64String(signText), hashAlgorithmName, rSAEncryptionPadding);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error Validando la Firma del Texto: {encryptText} con la Firma: {signText}", ex);
        }
    }

    /// <summary>
    /// Desencripta
    /// </summary>
    /// <param name="encryptText"></param>
    /// <returns></returns>
    public static string SignData(string privateKeyBase64, string data, HashAlgorithmName hashAlgorithmName, RSASignaturePadding rSAEncryptionPadding)
    {
        try
        {
            if (string.IsNullOrEmpty(privateKeyBase64))
                throw new InvalidOperationException("La llave Privada es requerida");
            var key = DecodeKey(privateKeyBase64);
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportFromPem(key.ToCharArray());
            var dataToSign = Encoding.UTF8.GetBytes(data);
            var signature = rsa.SignData(dataToSign, hashAlgorithmName, rSAEncryptionPadding);
            return Convert.ToBase64String(signature);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error mientras se intentaba firmar texto: {data} con el Hash: {hashAlgorithmName} y el padding :{rSAEncryptionPadding}", ex);
        }
    }

    /// <summary>
    /// Decodifica la Llave
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string DecodeKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key), "Llave vacía");
        try
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(key));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"No se pudo decodificar la llave: {key}", ex);
        }
    }
}
