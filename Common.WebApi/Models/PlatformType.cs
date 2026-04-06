using System.Runtime.Serialization;
namespace Common.WebApi.Models;

/// <summary>
/// Plataformas soportadas
/// </summary>
public enum PlatformType : byte
{
    /// <summary>
    /// Android
    /// </summary>
    [EnumMember(Value = "Android")]
    Android = 1,

    /// <summary>
    /// iOS
    /// </summary>
    [EnumMember(Value = "Ios")]
    iOS = 2,

    /// <summary>
    /// Web Browser
    /// </summary>
    [EnumMember(Value = "Web")]
    Web = 3
}