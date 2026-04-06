using System.Net.Http.Headers;
using Common.WebApi.ArtificialIntelligence.Gemini.Model;
using Common.WebApi.ArtificialIntelligence.Gemini.Model.Request;
using Common.WebApi.ArtificialIntelligence.Gemini.Model.Response;
using Common.WebApi.ArtificialIntelligence.Model.Common;
using Common.WebApi.ArtificialIntelligence.Model.Configuration;
using Common.WebApi.ArtificialIntelligence.Model.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Common.WebApi.ArtificialIntelligence.Gemini.Model.Request.GeminiProcessTextRequest;
namespace Common.WebApi.ArtificialIntelligence.Gemini;

/// <summary>
/// Implementacion para procesar texto en Gemini
/// </summary>
internal class GeminiArtificialIntelligence : ArtificialIntelligenceImplementationBase, IArtificialIntelligence
{
    /// <summary>
    /// Tipo de implementacion
    /// </summary>
    protected override ArtificialIntelligenceImplementationType ImplementationType => ArtificialIntelligenceImplementationType.Gemini;

    private readonly string _model;
    private readonly string _apiKey;

    /// <summary>
    /// Constructor de la implementacion
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="configuration"></param>
    /// <param name="keySelector">Selector de API Keys para round robin</param>
    /// <returns></returns>
    public GeminiArtificialIntelligence(
        ILogger<GeminiArtificialIntelligence> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    ) : base(
        logger,
        httpClientFactory,
        configuration)
    {
        var nationalCarrierConfiguration = Configuration.GetSection(nameof(ArtificialIntelligenceConfiguration))
           .Get<ArtificialIntelligenceConfiguration<GeminiConfiguration>>()
           ?? throw new ArtificialIntelligenceException("No se encontro las configuraciones de Artificial Intelligence");
        if (!nationalCarrierConfiguration.Implementations.TryGetValue($"{ImplementationType}", out var configurationModel))
            throw new ArtificialIntelligenceException($"No se encontro la configuracion de : {ImplementationType} ");
        ArgumentNullException.ThrowIfNull(configurationModel.BaseUrl);
        ArgumentNullException.ThrowIfNull(configurationModel.ApiKey);
        ArgumentNullException.ThrowIfNull(configurationModel.Model);
        _model = configurationModel.Model;
        _apiKey = configurationModel.ApiKey;
        HttpClient.BaseAddress = new Uri(configurationModel.BaseUrl);
        HttpClient.DefaultRequestHeaders.Accept.Clear();
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Obtiene el path de la API con la siguiente key en round robin
    /// </summary>
    private string GetApiPath() => $"models/{_model}:generateContent?key={_apiKey}";

    /// <summary>
    /// Procesa texto en Gemini
    /// </summary>
    /// <param name="request">Request de texto a procesar</param>
    /// <returns>Response de texto a procesar</returns>
    public async Task<string> ProcessTextAsync(ProcessTextRequest request)
    => await ExecuteAsync(async () =>
    {
        var geminiRequest = new GeminiProcessTextRequest
        {
            Contents = [new Content { Parts = [
                new () { Text = $"{request.Behavior}. {request.Indications}" },
            ] }],
            GenerationConfiguration = request.ResponseType switch
            {
                ProcessResponseType.Json => new GenerationConfig { ResponseMimeType = "application/json" },
                ProcessResponseType.Text => new GenerationConfig { ResponseMimeType = "text/plain" },
                _ => throw new ArtificialIntelligenceException($"Tipo de respuesta no soportada: {request.ResponseType}")
            }
        };
        var fullpath = GetApiPath();
        var responseContent = await SendPostModelAsync(fullpath, geminiRequest);
        var geminiResponse = JsonConvert.DeserializeObject<GeminiProcessTextResponse>(responseContent);
        var response = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        if (string.IsNullOrEmpty(response))
            throw new ArtificialIntelligenceException("No se pudo obtener una respuesta válida de Gemini");
        return response;
    });

}
