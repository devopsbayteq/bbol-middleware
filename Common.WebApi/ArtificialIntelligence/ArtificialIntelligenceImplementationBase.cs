using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.WebApi.ArtificialIntelligence;

internal abstract class ArtificialIntelligenceImplementationBase
{
    protected readonly ILogger<ArtificialIntelligenceImplementationBase> Logger;
    protected readonly IConfiguration Configuration;
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Constructor de la implementacions
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="httpClientFactory">HttpClientFactory</param>
    /// <param name="configuration">Configuracion</param>
    protected ArtificialIntelligenceImplementationBase(
        ILogger<ArtificialIntelligenceImplementationBase> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        Logger = logger;
        Configuration = configuration;
        HttpClient = httpClientFactory.CreateClient($"{ImplementationType}");
    }

    protected abstract ArtificialIntelligenceImplementationType ImplementationType { get; }

    /// <summary>
    /// Envia una solicitud a la API
    /// </summary>
    /// <typeparam name="TRequest">Tipo de request</typeparam>
    /// <param name="fullpath">Path a la API</param>
    /// <param name="processRequestModel">Request a enviar</param>
    /// <returns>Response de la API</returns>
    protected async Task<string> SendPostModelAsync<TRequest>(string fullpath, TRequest processRequestModel) where TRequest : class
    {
        var requestContent = JsonConvert.SerializeObject(processRequestModel);
        try
        {
            var response = await HttpClient.PostAsync(fullpath, new StringContent(requestContent, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return responseContent;
            throw new ArtificialIntelligenceException($"Error en respuesta del servicio: {responseContent}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al enviar la solicitud a la API: {@ImplementationType}", $"{ImplementationType}");
            throw new ArtificialIntelligenceException(ex.Message, ex);
        }
    }
    /// <summary>
    /// Ejecuta una solicitud a la API
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Response</returns>
    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> process)
    {
        try
        {
            return await process();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al ejecutar la solicitud a la API: {@ImplementationType}", $"{ImplementationType}");
            throw new ArtificialIntelligenceException(ex.Message, ex);
        }
    }
}
