namespace Common.Utils.Extensions;
/// <summary>
/// Extensión
/// </summary>
public static class LongExtensions
{
    /// <summary>
    /// Convierte en fecha desde una unidad de tiempo
    /// </summary>
    public static DateTimeOffset ToDateTimeOffSet(this long input)
    => input.ToString().Length >= 13
       ? DateTimeOffset.FromUnixTimeMilliseconds(input)
        : DateTimeOffset.FromUnixTimeSeconds(input);
}