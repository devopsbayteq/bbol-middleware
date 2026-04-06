using System.Runtime.Serialization;
namespace Common.WebApi.Extensions;
public static class EnumExtension
{

    /// <summary>
    /// Get the enum member value
    /// </summary>
    /// <param name="enum">The enum to get the member value</param>
    /// <returns>The enum member value</returns>
    /// </summary>
    public static string GetEnumMember(this Enum @enum)
    {
        var attr = @enum.GetType().GetMember(@enum.ToString()).FirstOrDefault()?
                                  .GetCustomAttributes(false).OfType<EnumMemberAttribute>()
                                  .FirstOrDefault();
        return attr == null ? @enum.ToString() : attr.Value;
    }
}