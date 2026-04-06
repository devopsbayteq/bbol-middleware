using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersistenceDb.CustomException;

namespace PersistenceDb.Utils.ValueConvertors;
/// <summary>
/// Convertidor de Valores
/// </summary>
public class EncryptionConvertor(ConverterMappingHints mappingHints = null)
: ValueConverter<string, string>(
    x => EncryptionExtension.Encrypt(x),
    x => EncryptionExtension.Decrypt(x),
     mappingHints)
{
}

/// <summary>
/// Clase de extensiòn para encriptar datos
/// </summary>
public static class EncryptionExtension
{
    public static string Key { get; private set; }

    /// <summary>
    /// Encripta la data
    /// </summary>
    /// <param name="dataToEncrypt"></param>
    /// <returns></returns>
    public static string Encrypt(string dataToEncrypt)
    {
        ValidateKey();
        if (string.IsNullOrEmpty(dataToEncrypt) || string.IsNullOrWhiteSpace(dataToEncrypt))
            throw new CustomPersistenceException("El texto a encriptar no puede estar vacío.");
        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(Key);
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(dataToEncrypt);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    /// <summary>
    /// Encriptor
    /// </summary>
    /// <param name="dataToDecrypt"></param>
    /// <returns></returns>
    public static string Decrypt(string dataToDecrypt)
    {
        ValidateKey();
        if (string.IsNullOrEmpty(dataToDecrypt) || string.IsNullOrWhiteSpace(dataToDecrypt))
            throw new CustomPersistenceException("El texto a desencriptar no puede estar vacío.");
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(Key);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        var descriptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var buffer = Convert.FromBase64String(dataToDecrypt);
        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, descriptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }

    /// <summary>
    /// Configura el Key para encripción
    /// </summary>
    /// <param name="key"></param>
    public static void SetEncryptionKey(string key)
    {
        Key = key;
        ValidateKey();
        if (key.Length < 16)
            throw new CustomPersistenceException("El secret de encripción debe tener al menos 16 caracteres");
    }

    /// <summary>
    /// Valida que esté configurado el secreto de encripción 
    /// </summary>
    private static void ValidateKey()
    {
        if (string.IsNullOrEmpty(Key))
            throw new CustomPersistenceException("El Key de encripción está vacío.");
    }
}