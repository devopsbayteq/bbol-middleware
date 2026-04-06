using Newtonsoft.Json.Serialization;

namespace Common.WebApi.Attributes.Json;

/// <summary>
/// Proveedor de valor para ignorar datos sensibles
/// </summary>
internal sealed class MaskingValueProvider(IValueProvider innerProvider, string mask) : IValueProvider
{
    private readonly IValueProvider _innerProvider = innerProvider;
    private readonly string _mask = mask;

    public object GetValue(object target)
    {
        var original = _innerProvider.GetValue(target);
        if (original == null) return null;

        // Mantener tipo string si aplica; para otros tipos devolver la misma máscara string
        return _mask;
    }

    public void SetValue(object target, object value)
    {
        _innerProvider.SetValue(target, value);
    }
}