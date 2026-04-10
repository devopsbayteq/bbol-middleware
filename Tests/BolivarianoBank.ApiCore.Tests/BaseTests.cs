using Common.WebApi.Extensions;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BolivarianoBank.ApiCore.Tests;

public class BaseTests
{
    protected TestServer Server;
    protected HttpClient Client;
    protected readonly JwtSettings Jwt;
    protected readonly RsaSecuritySettings RsaSecuritySettings;

    public BaseTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .Build();

        var jwtSettings = new JwtSettings();
        configuration.GetSection("Jwt").Bind(jwtSettings);
        Jwt = jwtSettings;

        var rsaSettings = new RsaSecuritySettings();
        configuration.GetSection("RsaSecurity").Bind(rsaSettings);
        RsaSecuritySettings = rsaSettings;
    }

    public static StringContent GenerateStringContent(object request) =>
        new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

    public readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public async Task<(HttpStatusCode, T)> GetResponse<T>(HttpClient client, HttpRequestMessage request)
    {
        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseContent))
            return (response.StatusCode, default);

        return (response.StatusCode, JsonSerializer.Deserialize<T>(responseContent, JsonOptions));
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(
        HttpMethod method,
        string url,
        object body,
        Dictionary<string, string> headers,
        bool addIntegrity,
        bool tokenRequired)
    {
        var request = new HttpRequestMessage(method, url);

        if (body is not null)
            request.Content = GenerateStringContent(body);

        headers ??= ValidateRootCompleteHeaders();

        if (addIntegrity)
        {
            var time = DateTimeOffset.Now.ToUnixTimeSeconds();
            headers["X-Time"] = time.ToString();

            headers.Add("X-Secret", Settings.SecretMock);

            //Parámetros del query
            var queryString = string.Empty;
            var uriString = request.RequestUri?.OriginalString;
            var queryIndex = uriString?.IndexOf('?') ?? 0;

            if (queryIndex >= 0 && queryIndex < uriString?.Length - 1)
                queryString = uriString?[(queryIndex + 1)..];

            //Cuerpo de la petición
            var hashBody = string.Empty;
            if (request.Content is not null)
                hashBody = (await request.Content.ReadAsStringAsync()).ToSha256();

            //Generamos contenido para comparación de integridad
            var nounce = $"{request.Method}||{queryString}||{hashBody}||{time}";
            var content = nounce.ToSha256(Settings.SecretDecryptMock);

            headers.Add("X-Content", content);
        }

        if (tokenRequired)
            headers.Add("Authorization", $"Bearer {GenerateAccessToken()}");

        if (headers is not null && headers.Count > 0)
            foreach (var header in headers)
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return request;
    }

    public async Task<(HttpStatusCode, T)> SendAsync<T>(HttpMethod method, string url, object body, Dictionary<string, string> headers = null, bool addIntegrity = true, bool tokenRequired = false)
    {
        var request = await CreateRequestAsync(method, url, body, headers, addIntegrity, tokenRequired).ConfigureAwait(false);
        return await GetResponse<T>(Client!, request).ConfigureAwait(false);
    }

    /// <summary>
    /// Misma petición que <see cref="SendAsync{T}"/>, pero devuelve el cuerpo sin deserializar (útil para 400 ProblemDetails).
    /// </summary>
    public async Task<(HttpStatusCode StatusCode, string RawContent)> SendAsyncRaw(
        HttpMethod method,
        string url,
        object body,
        Dictionary<string, string> headers = null,
        bool addIntegrity = true,
        bool tokenRequired = false)
    {
        var request = await CreateRequestAsync(method, url, body, headers, addIntegrity, tokenRequired).ConfigureAwait(false);
        using var response = await Client!.SendAsync(request).ConfigureAwait(false);
        var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return (response.StatusCode, raw ?? string.Empty);
    }

    protected static Dictionary<string, string> ConfigureContextCompleteHeaders()
    {
        return new Dictionary<string, string>
        {
            { "X-Timezone", "America/Guayaquil" },
            { "X-Platform", "Android" },
            { "X-Time", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() }
        };
    }

    protected static Dictionary<string, string> ValidateVersionCompleteHeaders()
    {
        var headers = ConfigureContextCompleteHeaders();
        headers.Add("X-Version", "1.0.0.1");
        return headers;
    }

    protected static Dictionary<string, string> ValidateRootCompleteHeaders()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers.Add("X-Fingerprint", Settings.FingerprintMock);
        return headers;
    }

    protected string GenerateAccessToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            Jwt.Issuer,
            Jwt.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(Jwt.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
