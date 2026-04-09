using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Transfer;
using BankCore.Integration.Models.User;
using BankCore.Integration.Security;
using BankCore.Integration.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace BolivarianoBank.ApiCore.Tests.Integration.Services;

/// <summary>
/// Pruebas unitarias para HttpsBankCoreServices
/// </summary>
public class HttpsBankCoreServicesTests : BaseTests
{
    protected Mock<IHttpClientFactory> HttpClientFactory;
    protected Mock<ITokenProvider> TokenProvider;
    protected Mock<ILogger<HttpsBankCoreServices>> Logger;
    protected Mock<HttpMessageHandler> HttpMessageHandler;
    protected HttpClient HttpClient;
    protected IBankCoreServices Service;

    [SetUp]
    public void Setup()
    {
        HttpClientFactory = new Mock<IHttpClientFactory>();
        TokenProvider = new Mock<ITokenProvider>();
        Logger = new Mock<ILogger<HttpsBankCoreServices>>();
        HttpMessageHandler = new Mock<HttpMessageHandler>();

        HttpClient = new HttpClient(HttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://api.bankcore.test/")
        };

        HttpClientFactory.Setup(f => f.CreateClient("BankCoreApiClient"))
            .Returns(HttpClient);

        TokenProvider.Setup(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("test-access-token");

        // Create service using reflection since it's internal
        Service = (IBankCoreServices)Activator.CreateInstance(
            Type.GetType("BankCore.Integration.Services.HttpsBankCoreServices, BankCore.Integration")!,
            HttpClientFactory.Object,
            TokenProvider.Object,
            Logger.Object)!;
    }

    [TearDown]
    public void TearDown()
    {
        HttpClient?.Dispose();
    }

    [Test]
    public async Task THBCS_01_GetAccessTokenAsync_ReturnsToken()
    {
        var expectedToken = "test-token-123";
        TokenProvider.Setup(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedToken);

        var token = await Service.GetAccessTokenAsync();

        Assert.Multiple(() =>
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.EqualTo(expectedToken));
        });

        TokenProvider.Verify(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task THBCS_02_ValidateUserAsync_Success()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123",
            Resultado = new()
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Codigo, Is.EqualTo("session123"));
            Assert.That(response.Resultado, Is.Not.Null);
        });
    }

    [Test]
    public async Task THBCS_03_ValidateUserPasswordAsync_Success()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var expectedResponse = new ValidateUserPasswordResponse
        {
            Codigo = "session123",
            Mensaje = string.Empty,
            Resultado = new()
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Codigo, Is.EqualTo("session123"));
            Assert.That(response.Resultado, Is.Not.Null);
        });
    }

    [Test]
    public async Task THBCS_04_TransferBetweenAccountsAsync_Success()
    {
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = DateTimeOffset.UtcNow,
            Tipo = "TRF"
        };

        var expectedResponse = new TransferBetweenAccountsResponse
        {
            SecuenciaTransaccion = "TXN-123456"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var response = await Service.TransferBetweenAccountsAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SecuenciaTransaccion, Is.EqualTo("TXN-123456"));
        });
    }

    [Test]
    public async Task THBCS_05_ValidateUserAsync_WithCancellationToken()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        using var cts = new CancellationTokenSource();
        var response = await Service.ValidateUserAsync(request, cts.Token);

        Assert.That(response, Is.Not.Null);
    }

    [Test]
    public void THBCS_06_ValidateUserAsync_InvalidJsonResponse_ThrowsException()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Invalid JSON Response")
            });

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await Service.ValidateUserAsync(request));
    }

    [Test]
    public async Task THBCS_07_TokenProvider_CalledForEachRequest()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.ValidateUserAsync(request);

        TokenProvider.Verify(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task THBCS_08_HttpClient_UsesCorrectEndpoint()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpRequestMessage capturedRequest = null!;

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest.RequestUri!.ToString(), Does.Contain("v1/adm-tarjeta/valida-usuarios"));
            Assert.That(capturedRequest.Method, Is.EqualTo(HttpMethod.Post));
        });
    }

    [Test]
    public async Task THBCS_09_HttpClient_IncludesAuthorizationHeader()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpRequestMessage capturedRequest = null!;

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest.Headers.Authorization, Is.Not.Null);
            Assert.That(capturedRequest.Headers.Authorization!.Scheme, Is.EqualTo("Bearer"));
            Assert.That(capturedRequest.Headers.Authorization.Parameter, Is.EqualTo("test-access-token"));
        });
    }

    [Test]
    public async Task THBCS_10_HttpClient_IncludesContentTypeHeader()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpRequestMessage capturedRequest = null!;

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest.Content, Is.Not.Null);
            Assert.That(capturedRequest.Content!.Headers.ContentType!.MediaType, Is.EqualTo("application/json"));
        });
    }

    [Test]
    public async Task THBCS_11_ValidatePasswordAsync_CorrectEndpoint()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var expectedResponse = new ValidateUserPasswordResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpRequestMessage capturedRequest = null!;

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.ValidateUserPasswordAsync(request);

        Assert.That(capturedRequest.RequestUri!.ToString(), Does.Contain("v1/adm-tarjeta/valida-clave-usuarios"));
    }

    [Test]
    public async Task THBCS_12_TransferAsync_CorrectEndpoint()
    {
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = DateTimeOffset.UtcNow,
            Tipo = "TRF"
        };

        var expectedResponse = new TransferBetweenAccountsResponse
        {
            SecuenciaTransaccion = "TXN-123456"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpRequestMessage capturedRequest = null!;

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.TransferBetweenAccountsAsync(request);

        Assert.That(capturedRequest.RequestUri!.ToString(), Does.Contain("v1/adm-tarjeta/transferencias-entre-cuentas"));
    }

    [Test]
    public void THBCS_13_TransferAsync_InvalidResponse_ThrowsException()
    {
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = DateTimeOffset.UtcNow,
            Tipo = "TRF"
        };

        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{invalid json}")
            });

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await Service.TransferBetweenAccountsAsync(request));

        Assert.That(ex.Message, Does.Contain("Error al deserializar la respuesta de la API externa"));
    }    

    [Test]
    public async Task THBCS_14_HttpClientFactory_CreatesClientWithCorrectName()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");
        var expectedResponse = new ValidateUserResponse
        {
            Codigo = "session123"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        HttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        await Service.ValidateUserAsync(request);

        HttpClientFactory.Verify(f => f.CreateClient("BankCoreApiClient"), Times.AtLeastOnce);
    }
}
