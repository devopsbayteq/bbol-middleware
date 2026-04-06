namespace Common.WebApi.Attributes;

/// <summary>
/// Marca propiedades sensibles para serialización o logs.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreSensibleAttribute : Attribute;
