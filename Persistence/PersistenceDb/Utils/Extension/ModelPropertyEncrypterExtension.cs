using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceDb.Models.Attributes;
using PersistenceDb.Utils.ValueConvertors;

namespace PersistenceDb.Utils.Extension;

public static class ModelPropertyEncrypterExtension
{
    /// <summary>
    /// Extensión para usar Encripción
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="secretAes"></param>
    public static void UseEncryption(this ModelBuilder modelBuilder, string secretAes)
    {
        EncryptionExtension.SetEncryptionKey(secretAes);
        // Instantiate the EncryptionConverter
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(select => select.GetProperties())
            .Where(where => where.ClrType == typeof(string) && !IsDiscriminator(where) && where.PropertyInfo.GetCustomAttributes(typeof(EncryptColumnAttribute), false).Length != 0))
            property.SetValueConverter(new EncryptionConvertor());
    }
    /// <summary>
    /// Discriminador
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private static bool IsDiscriminator(IMutableProperty property) => property.Name == "Discriminator" || property.PropertyInfo == null;
}