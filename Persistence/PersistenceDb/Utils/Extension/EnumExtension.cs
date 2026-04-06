using System.Runtime.Serialization;

namespace PersistenceDb.Utils.Extension;

/// <summary>
/// Extensiones para enumerables de PersistenceDb
/// </summary>
internal static class PersistenceDbEnumExtension
{
    /// <summary>
    /// Obtiene el Enumerable
    /// </summary>
    internal static string GetEnumMember(this Enum @enum)
    {
        var attr = @enum.GetType().GetMember(@enum.ToString()).FirstOrDefault()?
                                  .GetCustomAttributes(false).OfType<EnumMemberAttribute>()
                                  .FirstOrDefault();
        return attr == null ? @enum.ToString() : attr.Value;
    }
}