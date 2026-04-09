using ApiCore.Models;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using LogicApi.Model.Request.Authentication;
using LogicApi.Model.Response.Authentication;
using Moq;
using System.Net;
using System.Text.Json;

namespace BolivarianoBank.ApiCore.Tests.Controllers;

/// <summary>
/// Pruebas unitarias para Controlador Authentication
/// </summary>
public partial class ControllerTests
{
    [Test]
    public async Task TA_01_LoginSuccessful()
    {
        var requestBody = new LoginRequest
        {
            Username = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
        };

        var loginResponse = new LoginResponse
        {
            AccessToken = Guid.NewGuid().ToString()
        };

        Mediator.Setup(m => m.Send(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(loginResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<LoginResponse>>(HttpMethod.Post, Settings.LoginUrl, body: requestBody);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.Success));
            Assert.That(response.Message, Is.EqualTo(MessageCodes.Success.GetEnumMember()));
            Assert.That(response.Content.AccessToken, Is.EqualTo(loginResponse.AccessToken));
        });
    }

    [Test]
    public async Task TA_02_LoginBiometricRequestError()
    {
        var requestBody = new BiometricLoginRequest();

        (var statusCode, var response) = await SendAsync<object>(HttpMethod.Post, Settings.BiometricLoginUrl, body: requestBody);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response, Is.Not.Null);
            Assert.That(JsonSerializer.Serialize(response), Does.Contain("field is required"));
        });
    }

    [Test]
    public async Task TA_03_LoginBiometricSuccessful()
    {
        var requestBody = new BiometricLoginRequest
        {
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Guid.NewGuid().ToString(),
            UsernameEncryptBase64 = Guid.NewGuid().ToString(),
        };

        var loginResponse = new LoginResponse
        {
            AccessToken = Guid.NewGuid().ToString()
        };

        Mediator.Setup(m => m.Send(It.IsAny<BiometricLoginRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(loginResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<LoginResponse>>(HttpMethod.Post, Settings.BiometricLoginUrl, body: requestBody);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.Success));
            Assert.That(response.Message, Is.EqualTo(MessageCodes.Success.GetEnumMember()));
            Assert.That(response.Content.AccessToken, Is.EqualTo(loginResponse.AccessToken));
        });
    }
}
