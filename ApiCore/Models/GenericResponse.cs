using System.Text.Json;

namespace ApiCore.Models;

/// <summary>
/// Respuesta Genérica
/// </summary>
public class GenericResponse
{
    /// <summary>
    /// Código de respuesta
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Mensaje al usuario
    /// </summary>
    public string Message { get; set; }
}

/// <summary>
/// Respuesta Genérica
/// </summary>
public class GenericResponse<T>
{
    /// <summary>
    /// Instancia estática de opcinoes
    /// </summary>
    /// <returns></returns>
    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Código de respuesta
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Response type
    /// </summary>
    public string ResponseType { get; set; }

    /// <summary>
    /// Mensaje al usuario
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Información de vuelta al usuario
    /// </summary>
    public T Content { get; set; }

    /// <summary>
    /// Información adicional
    /// </summary>
    public string AdditionalData { get; set; }

    /// <summary>
    /// Get a serialized Generic response class
    /// </summary>
    /// <returns></returns>
    public override string ToString() => JsonSerializer.Serialize(this, JsonSerializerOptions);
}
