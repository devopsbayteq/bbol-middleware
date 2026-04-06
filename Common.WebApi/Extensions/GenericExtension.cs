using Newtonsoft.Json;
namespace Common.WebApi.Extensions;
/// <summary>
/// Genérico
/// </summary>
public static class GenericExtension
{
    /// <summary>
    /// Transforma objeto a Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static string ToJson<T>(this T input) where T : class
        => JsonConvert.SerializeObject(input);
}