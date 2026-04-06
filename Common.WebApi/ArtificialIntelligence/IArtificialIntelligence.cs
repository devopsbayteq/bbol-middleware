using Common.WebApi.ArtificialIntelligence.Model.Request;

namespace Common.WebApi.ArtificialIntelligence;

/// <summary>
/// Interfaz para consultas sobre la IA
/// </summary>
public interface IArtificialIntelligence
{
    /// <summary>
    /// Procesa texto con Inteligencia Artificial
    /// </summary>  
    /// <param name="request">Request de texto a procesar</param>
    /// <returns>Response de texto a procesar</returns>
    Task<string> ProcessTextAsync(ProcessTextRequest request);

  
}
