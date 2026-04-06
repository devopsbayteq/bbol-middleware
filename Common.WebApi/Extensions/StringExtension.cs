using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
namespace Common.WebApi.Extensions;

/// <summary>
/// String Extension
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Verifica si es null or empty y envía excepción
    /// </summary>
    /// <param name="input"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string input, Exception exception = null)
    {
        var isNullOrEmpty = string.IsNullOrEmpty(input);
        if (isNullOrEmpty && exception is not null)
            throw exception;
        return isNullOrEmpty;
    }

    /// <summary>
    /// Decodifica un String
    /// </summary>
    /// <param name="encodeStringInput"></param>
    /// <returns></returns>
    public static string Decode(this string encodeStringInput, bool removeNewLine = true)
    {
        var decodeString = Encoding.UTF8.GetString(Base64ToBytes(encodeStringInput));
        if (removeNewLine)
            decodeString = decodeString.Replace("\n", "");
        return decodeString;
    }

    /// <summary>
    /// Codifica un String
    /// </summary>
    /// <param name="encodeStringInput"></param>
    /// <returns></returns>
    public static string Encode(this string decodeStringInput)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(decodeStringInput);
        return Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Convierte la Json a Objeto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static T ToObject<T>(this string source)
    => JsonConvert.DeserializeObject<T>(source);

    /// <summary>
    /// Codifica un String a Bytes
    /// </summary>
    /// <param name="encodeStringInput"></param>
    /// <returns></returns>
    public static byte[] Base64ToBytes(this string encodeStringInput)
    {
        if (string.IsNullOrEmpty(encodeStringInput))
            throw new FormatException("No se puede decodificar un string null o vacío.");
        if (encodeStringInput.Length % 4 != 0)
            throw new FormatException($"La cadena '{encodeStringInput}' no tiene la cantidad de caracteres adecuados para ser base64. (%4)");
        if (encodeStringInput.Contains(' ') || encodeStringInput.Contains('\t') || encodeStringInput.Contains('\r') || encodeStringInput.Contains('\n'))
            throw new FormatException($"La cadena '{encodeStringInput}' tiene espacios en blanco, saltos de lineas u otro caracter especial.");
        return Convert.FromBase64String(encodeStringInput);
    }

    /// <summary>
    /// Calcula Hash sha256
    /// </summary>
    /// <param name="secretKey"></param>
    /// <param name="dataInput"></param>
    /// <param name="base64Output"></param>
    /// <returns></returns>
    public static string ToSha256(this string dataInput, bool base64Output = false)
    {
        var encoder = new UTF8Encoding();
        var byteDataInput = encoder.GetBytes(dataInput);
        var hmacResult = SHA256.HashData(byteDataInput);
        //Devuelve en Base64
        if (base64Output)
            return hmacResult.ToBase64();
        //Devuelve en HEX
        var sb = new StringBuilder();
        foreach (var t in hmacResult)
            sb.Append(t.ToString("X2"));
        return sb.ToString().ToLower();
    }

    /// <summary>
    /// Calcula Hash MACSHA256 con una Clave Secreta
    /// </summary>
    /// <param name="secretKey"></param>
    /// <param name="dataInput"></param>
    /// <param name="base64Output"></param>
    /// <returns></returns>
    public static string ToSha256(this string dataInput, string secretKey, bool base64Output = false)
    {
        var encoder = new UTF8Encoding();
        var byteSecret = encoder.GetBytes(secretKey);
        var byteDataInput = encoder.GetBytes(dataInput);

        var hmac = new HMACSHA256(byteSecret);
        var hmacResult = hmac.ComputeHash(byteDataInput);

        //Devuelve en Base64
        if (base64Output)
            return hmacResult.ToBase64();

        //Devuelve en HEX
        var sb = new StringBuilder();
        foreach (var t in hmacResult)
            sb.Append(t.ToString("X2"));

        return sb.ToString().ToLower();
    }
}
