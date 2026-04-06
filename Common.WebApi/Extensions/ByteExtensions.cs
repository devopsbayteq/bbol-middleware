namespace Common.WebApi.Extensions;
public static class ByteExtensions
{
    /// <summary>
    /// Convierte a Base 64
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    public static string ToBase64(this byte[] byteArray)
        => Convert.ToBase64String(byteArray);

    /// <summary>
    /// Convierte un arreglo de Bytes en Stream
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>    
    public static Stream ToStream(this byte[] byteArray)
    => new MemoryStream(byteArray) { Position = 0 };

    /// <summary>
    /// Convierte un Byte a Hexadecimal
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="upperCase"></param>
    /// <returns></returns>
    public static string ToHexString(this IEnumerable<byte> bytes, bool upperCase = true) => bytes.Aggregate("", (current, t) => current + t.ToString(upperCase ? "X2" : "x2"));

}
