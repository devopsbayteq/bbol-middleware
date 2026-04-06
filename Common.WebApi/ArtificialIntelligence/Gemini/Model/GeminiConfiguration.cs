using Common.WebApi.ArtificialIntelligence.Model.Configuration;

namespace Common.WebApi.ArtificialIntelligence.Gemini.Model;

/// <summary>
/// Configuración de Gemini
/// </summary>
public class GeminiConfiguration : ArtificialIntelligenceImplementation
{
    /// <summary>
    /// Path para la generación de texto
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// Api Key de Gemini (legacy - usar ApiKeys para múltiples keys)
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Modelo de Gemini
    /// </summary>
    public string Model { get; set; }
}