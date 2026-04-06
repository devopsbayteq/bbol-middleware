using System.Security.Cryptography;
using System.Text;
using Common.WebApi.Extensions;

namespace Common.WebApi.Security;

public static class AesSecurity
{
    /// <summary>
    /// Encrypta con AES
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string EncryptAesToHex(string plainText, string key, string iv)
    {
        using var aes = Aes.Create();
        var encryptor = aes.CreateEncryptor(Encoding.Default.GetBytes(key), Encoding.Default.GetBytes(iv));
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        using (StreamWriter sw = new(cs))
            sw.Write(plainText);
        var encrypted = ms.ToArray();
        return encrypted.ToHexString();
    }

    /// <summary>
    /// Encripta información con AES
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string EncryptAes(string plainText, string key, string iv = null)
    {
         //Validaciones
        ArgumentException.ThrowIfNullOrEmpty(plainText);
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (key.Length < 15)
            throw new ArgumentException("La llave debe tener al menos 15 caracteres", nameof(key));
        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        if (!iv.IsNullOrEmpty())
        {
            ArgumentException.ThrowIfNullOrEmpty(iv);
            if (iv.Length < 15)
                throw new ArgumentException("El IV debe tener al menos 15 caracteres", nameof(iv));
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
        }
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;
        //Creamos el encriptro
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    /// <summary>
    /// Desencripta Información con AES
    /// </summary>
    /// <param name="dataToDecrypt"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string DecryptAes(string dataToDecrypt, string key, string iv = null)
    {
        //Validaciones
        ArgumentException.ThrowIfNullOrEmpty(dataToDecrypt);
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (key.Length < 15)
            throw new ArgumentException("La llave debe tener al menos 15 caracteres", nameof(key));
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        if (!iv.IsNullOrEmpty())
        {
            ArgumentException.ThrowIfNullOrEmpty(iv);
            if (iv.Length < 15)
                throw new ArgumentException("El IV debe tener al menos 15 caracteres", nameof(iv));
            aes.IV = Encoding.UTF8.GetBytes(iv);
        }
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        var descriptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var buffer = Convert.FromBase64String(dataToDecrypt);
        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, descriptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }
}