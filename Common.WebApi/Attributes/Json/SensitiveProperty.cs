using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Common.WebApi.Attributes.Json;

public class SensitiveProductionProperty : DefaultContractResolver
{
    protected override List<MemberInfo> GetSerializableMembers(Type objectType) => [.. objectType.GetProperties()
            .Where(pi => !Attribute.IsDefined(pi, typeof(JsonIgnoreAttribute)))];
    protected override JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (member.GetCustomAttribute<JsonCompresseAttribute>() != null)
            property.ValueProvider = new JsonCompresseValueProvider(property.ValueProvider);
        if (member.GetCustomAttribute<IgnoreSensibleAttribute>() != null)
        {
            if (property.PropertyType == typeof(string))
                property.ValueProvider = new MaskingValueProvider(property.ValueProvider, "*****");
            else
                property.ValueProvider = new MaskingValueProvider(property.ValueProvider, null);
        }
        return property;
    }
}

public class SensitiveDevelopmentProperty : DefaultContractResolver
{
    protected override List<MemberInfo> GetSerializableMembers(Type objectType) => [.. objectType.GetProperties()
    .Where(pi => !Attribute.IsDefined(pi, typeof(JsonIgnoreAttribute)))];


    protected override JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (member.GetCustomAttribute<JsonCompresseAttribute>() != null)
            property.ValueProvider = new JsonCompresseValueProvider(property.ValueProvider);
        if (member.GetCustomAttribute<IgnoreSensibleAttribute>() != null)
        {
            if (property.PropertyType == typeof(string))
                property.ValueProvider = new MaskingValueProvider(property.ValueProvider, "*****");
            else
                property.ValueProvider = new MaskingValueProvider(property.ValueProvider, null);
        }
        return property;
    }
}