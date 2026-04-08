using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BankCore.Integration.Exceptions;
using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Common;
using BankCore.Integration.Models.Transfer;
using BankCore.Integration.Models.User;
using BankCore.Integration.Security;
using Common.WebApi.Attributes.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BankCore.Integration.Services;

internal sealed class HttpsBankCoreServices(
    IHttpClientFactory httpClientFactory,
    ITokenProvider tokenProvider,
    ILogger<HttpsBankCoreServices> logger
    ) : IBankCoreServices
{
    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        => tokenProvider.GetAccessTokenAsync(cancellationToken);

    public Task<ValidateUserResponse> ValidateUserAsync(ValidateUserRequest request, CancellationToken cancellationToken = default)
        => PostAsync<ValidateUserRequest, ValidateUserResponse>("v1/adm-tarjeta/valida-usuarios", request, cancellationToken, true);

    public Task<ValidateUserPasswordResponse> ValidateUserPasswordAsync(ValidateUserPasswordRequest request, CancellationToken cancellationToken = default)
        => PostAsync<ValidateUserPasswordRequest, ValidateUserPasswordResponse>("v1/adm-tarjeta/valida-clave-usuarios", request, cancellationToken, false);

    public Task<TransferBetweenAccountsResponse> TransferBetweenAccountsAsync(TransferBetweenAccountsRequest request, CancellationToken cancellationToken = default)
        => PostAsync<TransferBetweenAccountsRequest, TransferBetweenAccountsResponse>("v1/adm-tarjeta/transferencias-entre-cuentas", request, cancellationToken);

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string relativePath,
        TRequest request,
        CancellationToken cancellationToken,
        bool useCaseSensitive = false) where TResponse : class
    {
        var token = await tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var client = httpClientFactory.CreateClient("BankCoreApiClient");
        var payloadLogger = JsonConvert.SerializeObject(request, RequestJsonSerializerSettings);
        var payloadRequest = JsonConvert.SerializeObject(request, JsonSerializerSettings(useCaseSensitive));
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativePath)
        {
            Content = new StringContent(payloadRequest, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Request BankCore {@RelativePath}: {@Request}", relativePath, payloadLogger);
        using var response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Response BankCore {@RelativePath}: {@StatusCode}", relativePath, response.StatusCode);
        try
        {
            if (!response.IsSuccessStatusCode)
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Error response BankCore: {@Raw}", raw);
                var errorResponse = JsonConvert.DeserializeObject<BankCoreErrorResponse>(raw);
                throw new BankCoreUserMessageException(errorResponse?.UserMessage ?? "No se pudo procesar tu transaccion. Por favor, contacta a 550-5050.");
            }
            var model = JsonConvert.DeserializeObject<TResponse>(raw);
            var jsonResponse = JsonConvert.SerializeObject(model, RequestJsonSerializerSettings);
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Response BankCore {@RelativePath}: {@Model}", relativePath, jsonResponse);
            return model;
        }
        catch (BankCoreUserMessageException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al deserializar la respuesta de la API externa: {@RelativePath} - {@Raw}", relativePath, raw);
            throw new InvalidOperationException("Error al deserializar la respuesta de la API externa.", ex);
        }
    }

    /// <summary>
    /// Request al core: nombres de propiedad en camelCase; mantiene reglas de datos sensibles.
    /// </summary>
    private static JsonSerializerSettings RequestJsonSerializerSettings => new()
    {
        ContractResolver = new SensitiveProductionProperty
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    /// <summary>
    /// Serialización del cuerpo hacia Bank Core: true = camelCase, false = PascalCase. Sin enmascarado de datos sensibles.
    /// </summary>
    private static JsonSerializerSettings JsonSerializerSettings(bool useCaseSensitive) => new()
    {
        ContractResolver = useCaseSensitive
            ? new CamelCasePropertyNamesContractResolver()
            : new DefaultContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
}
