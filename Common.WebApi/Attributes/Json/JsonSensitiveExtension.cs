using Newtonsoft.Json;
namespace Common.WebApi.Attributes.Json;
/// <summary>
/// Json
/// </summary>
public static class JsonSensitiveExtension
{
    /// <summary>
    /// Transforma objeto a Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static string ToJsonIgnore<T>(this T input, bool ignoreReferenceLoopHandling = true) where T : class
        => JsonConvert.SerializeObject(input,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ignoreReferenceLoopHandling ? ReferenceLoopHandling.Ignore : ReferenceLoopHandling.Error,
                ContractResolver = new SensitiveProductionProperty()
            });
}