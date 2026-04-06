namespace PersistenceDb.Models.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
/// <summary>
/// Atributo para encriptar
/// </summary>
public class EncryptColumnAttribute : Attribute
{
}