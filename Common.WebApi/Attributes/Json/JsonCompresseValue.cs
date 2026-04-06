using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Common.WebApi.Attributes.Json;
public class JsonCompresseValue : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (member.GetCustomAttribute<JsonCompresseAttribute>() != null)
            property.ValueProvider = new JsonCompresseValueProvider(property.ValueProvider);
        return property;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class JsonCompresseAttribute : Attribute
{

}

/// <summary>
/// Proveedor de valor para comprimir el valor de la propiedad
/// </summary>
public sealed class JsonCompresseValueProvider(IValueProvider innerProvider) : IValueProvider
{
    private readonly IValueProvider _innerProvider = innerProvider;
    private const int FirstChars = 10;
    private const int LastChars = 10;
    private const string Separator = ".....";

    /// <summary>
    /// Obtiene el valor de la propiedad comprimido (primeros 10 caracteres + ..... + últimos 10 caracteres)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public object GetValue(object target)
    {
        var original = _innerProvider.GetValue(target);
        if (original == null) return null;

        var valueString = original.ToString();
        if (string.IsNullOrEmpty(valueString)) return valueString;

        // Si el valor es menor o igual a la longitud total (primeros + separador + últimos), devolver completo
        var totalLength = FirstChars + Separator.Length + LastChars;
        if (valueString.Length <= totalLength)
            return valueString;

        // Comprimir: primeros 10 + ..... + últimos 10
        var firstPart = valueString[..FirstChars];
        var lastPart = valueString[^LastChars..];
        return $"{firstPart}{Separator}{lastPart}";
    }

    /// <summary>
    /// Establece el valor de la propiedad
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></returns>
    public void SetValue(object target, object value)
    {
        _innerProvider.SetValue(target, value);
    }
}