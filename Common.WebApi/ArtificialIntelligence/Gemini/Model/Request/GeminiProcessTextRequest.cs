using Newtonsoft.Json;

namespace Common.WebApi.ArtificialIntelligence.Gemini.Model.Request;

/// <summary>
/// Request para procesar texto en Gemini
/// </summary>
internal class GeminiProcessTextRequest
{
    /// <summary>
    /// Contenidos a generar
    /// </summary>
    [JsonProperty("contents")]
    public Content[] Contents { get; set; }

    /// <summary>
    /// Configuracion de la generacion
    /// </summary>
    [JsonProperty("generationConfig")]
    public GenerationConfig GenerationConfiguration { get; set; }

    /// <summary>
    /// Partes del contenido
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Partes del contenido
        /// </summary>
        [JsonProperty("parts")]
        public Part[] Parts { get; set; }
    }

    /// <summary>
    /// Partes del contenido
    /// </summary>
    public class Part
    {
        /// <summary>
        /// Texto a generar
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        /// <summary>
        /// Datos inline
        /// </summary>
        [JsonProperty("inline_data", NullValueHandling = NullValueHandling.Ignore)]
        public InlineData InlineData { get; set; }
    }

    /// <summary>
    /// Datos inline
    /// </summary>
    public class InlineData
    {
        /// <summary>
        /// Tipo de mime
        /// </summary>
        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Datos
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }

    /// <summary>
    /// Configuracion de la generacion
    /// </summary>
    public class GenerationConfig
    {
        /// <summary>
        /// Tipo de mime de la respuesta
        /// </summary>
        [JsonProperty("response_mime_type")]
        public string ResponseMimeType { get; set; }
    }

}