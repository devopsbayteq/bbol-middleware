using Newtonsoft.Json;

namespace Common.WebApi.ArtificialIntelligence.Gemini.Model.Response;

/// <summary>
/// Response para procesar texto en Gemini
/// </summary>
internal class GeminiProcessTextResponse
{
    /// <summary>
    /// Candidatos
    /// </summary>
    [JsonProperty("candidates")]
    public Candidate[] Candidates { get; set; }

    /// <summary>
    /// Informacion de uso
    /// </summary>
    [JsonProperty("usageMetadata")]
    public UsageMetadata UsageMetadataInformation { get; set; }

    /// <summary>
    /// Modelo de la version
    /// </summary>
    [JsonProperty("modelVersion")]
    public string ModelVersion { get; set; }

    /// <summary>
    /// Id de la respuesta
    /// </summary>
    [JsonProperty("responseId")]
    public string ResponseId { get; set; }

    /// <summary>
    /// Candidato
    /// </summary>
    public class Candidate
    {
        /// <summary>
        /// Contenido
        /// </summary>
        [JsonProperty("content")]
        public Content Content { get; set; }

        /// <summary>
        /// Razon de finalizacion
        /// </summary>
        [JsonProperty("finishReason")]
        public string FinishReason { get; set; }

        /// <summary>
        /// Promedio de logprobs
        /// </summary>
        [JsonProperty("avgLogprobs")]
        public double AvgLogprobs { get; set; }
    }

    /// <summary>
    /// Contenido
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Partes
        /// </summary>
        [JsonProperty("parts")]
        public Part[] Parts { get; set; }

        /// <summary>
        /// Rol
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; }
    }

    /// <summary>
    /// Parte
    /// </summary>
    public class Part
    {
        /// <summary>
        /// Texto
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// Informacion de uso
    /// </summary>
    public class UsageMetadata
    {
        /// <summary>
        /// Promp de tokens
        /// </summary>
        [JsonProperty("promptTokenCount")]
        public long PromptTokenCount { get; set; }

        /// <summary>
        /// Tokens de candidatos
        /// </summary>
        [JsonProperty("candidatesTokenCount")]
        public long CandidatesTokenCount { get; set; }

        /// <summary>
        /// Total de tokens
        /// </summary>
        [JsonProperty("totalTokenCount")]
        public long TotalTokenCount { get; set; }

        /// <summary>
        /// Detalles de tokens de prompt
        /// </summary>
        [JsonProperty("promptTokensDetails")]
        public TokensDetail[] PromptTokensDetails { get; set; }

        /// <summary>
        /// Detalles de tokens de candidatos
        /// </summary>
        [JsonProperty("candidatesTokensDetails")]
        public TokensDetail[] CandidatesTokensDetails { get; set; }
    }

    /// <summary>
    /// Detalles de tokens
    /// </summary>
    public class TokensDetail
    {
        /// <summary>
        /// Modo
        /// </summary>
        [JsonProperty("modality")]
        public string Modality { get; set; }

        /// <summary>
        /// Total de tokens
        /// </summary>
        [JsonProperty("tokenCount")]
        public long TokenCount { get; set; }
    }
}