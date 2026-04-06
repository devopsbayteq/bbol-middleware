using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Common;
using BankCore.Integration.Models.Transfer;
using BankCore.Integration.Models.User;
using BankCore.Integration.Security;
using Common.WebApi.Attributes.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        => PostAsync<ValidateUserRequest, ValidateUserResponse>("v1/adm-tarjeta/valida-usuarios", request, cancellationToken);

    public Task<ValidateUserPasswordResponse> ValidateUserPasswordAsync(ValidateUserPasswordRequest request, CancellationToken cancellationToken = default)
        => PostAsync<ValidateUserPasswordRequest, ValidateUserPasswordResponse>("v1/adm-tarjeta/valida-clave-usuarios", request, cancellationToken);

    public Task<TransferBetweenAccountsResponse> TransferBetweenAccountsAsync(TransferBetweenAccountsRequest request, CancellationToken cancellationToken = default)
        => PostAsync<TransferBetweenAccountsRequest, TransferBetweenAccountsResponse>("v1/adm-tarjeta/transferencias-entre-cuentas", request, cancellationToken);

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string relativePath, TRequest request, CancellationToken cancellationToken) where TResponse : class
    {
        var token = await tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var client = httpClientFactory.CreateClient("BankCoreApiClient");
        var payload = JsonConvert.SerializeObject(request, JsonSerializerSettings);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativePath)
        {
            Content = new StringContent(payload, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var jsonRequest = JsonConvert.SerializeObject(request, JsonSerializerSettings);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Request BankCore {@RelativePath}: {@Request}", relativePath, jsonRequest);
        using var response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Response BankCore {@RelativePath}: {@StatusCode}", relativePath, response.StatusCode);
        try
        {
            var model = JsonConvert.DeserializeObject<TResponse>(raw);
            var jsonResponse = JsonConvert.SerializeObject(model, JsonSerializerSettings);
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Response BankCore {@RelativePath}: {@Model}", relativePath, jsonResponse);
            return model;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al deserializar la respuesta de la API externa: {@RelativePath} - {@Raw}", relativePath, raw);
            throw new InvalidOperationException("Error al deserializar la respuesta de la API externa.", ex);
        }
    }

    private static JsonSerializerSettings JsonSerializerSettings => new()
    {
        ContractResolver = new SensitiveProductionProperty(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
}
