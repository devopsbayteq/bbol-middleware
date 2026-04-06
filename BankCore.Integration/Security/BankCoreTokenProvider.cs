using System.Net.Http.Headers;
using System.Text.Json;
using BankCore.Integration.Models;
using BankCore.Integration.Models.Auth;
using Microsoft.Extensions.Options;

namespace BankCore.Integration.Security;

internal sealed class BankCoreTokenProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<BankCoreOptions> options
    ) : ITokenProvider
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);
    private string _accessToken = string.Empty;
    private DateTimeOffset _expiresAtUtc = DateTimeOffset.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (IsCurrentTokenValid())
            return _accessToken;

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsCurrentTokenValid())
                return _accessToken;

            var token = await RequestTokenAsync(cancellationToken).ConfigureAwait(false);
            _accessToken = token.AccessToken;
            _expiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private bool IsCurrentTokenValid()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
            return false;

        var refreshBefore = TimeSpan.FromSeconds(Math.Max(0, options.Value.TokenRefreshBeforeExpirySeconds));
        return DateTimeOffset.UtcNow < _expiresAtUtc.Subtract(refreshBefore);
    }

    private async Task<TokenResponse> RequestTokenAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("BankCoreAuthClient");
        var request = new HttpRequestMessage(HttpMethod.Post, string.Empty)
        {
            Content = new FormUrlEncodedContent(
            [
                new("grant_type", "client_credentials"),
                new("scope", options.Value.Scope),
                new("client_id", options.Value.ClientId),
                new("client_secret", options.Value.ClientSecret)
            ])
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"No se pudo obtener token OAuth. Status={(int)response.StatusCode}; Body={content}");

        var model = JsonSerializer.Deserialize<TokenResponse>(content, _serializerOptions)
            ?? throw new InvalidOperationException("Respuesta de token vacía o inválida.");
        if (string.IsNullOrWhiteSpace(model.AccessToken))
            throw new InvalidOperationException("La respuesta de token no contiene access_token.");
        if (model.ExpiresIn <= 0)
            model.ExpiresIn = 300;
        return model;
    }
}
