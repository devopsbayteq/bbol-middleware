namespace Common.WebApi.ArtificialIntelligence.Model.Configuration;
/// <summary>
/// Configuración de la Artificial Intelligence
/// </summary>
public class ArtificialIntelligenceConfiguration<T> : ArtificialIntelligenceConfiguration where T : ArtificialIntelligenceImplementation
{
    /// <summary>
    /// Implementaciones disponibles
    /// </summary>
    public Dictionary<string, T> Implementations { get; set; }
}

/// <summary>
/// Configuración base de la Artificial Intelligence
/// </summary>
public class ArtificialIntelligenceConfiguration
{
    /// <summary>
    /// Implementación actual
    /// </summary>
    /// <value></value>
    public string CurrentImplementation { get; set; }
}

/// <summary>
/// Configuración de la implementación de la Artificial Intelligence
/// </summary>
public abstract class ArtificialIntelligenceImplementation
{
    /// <summary>
    /// Tipo de implementación
    /// </summary>
    public string ImplementationName { get; set; }
}