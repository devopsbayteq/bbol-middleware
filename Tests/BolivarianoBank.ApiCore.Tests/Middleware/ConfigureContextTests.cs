using ApiCore.Models;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Middleware;

/// <summary>
/// Pruebas unitarias para Middleware ConfigureContext
/// </summary>
public partial class MiddlewareTest
{
    [Test]
    public async Task TCC_01_GenericExceptionByTimeZoneRequired()
    {
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: [], addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.SystemError));
            Assert.That(response.Message, Is.EqualTo(MessageCodes.SystemError.GetEnumMember()));
            Assert.That(response.AdditionalData, Is.Null);
            Assert.That(response.Content, Is.Not.Null);
        });
    }

    [Test]
    public async Task TCC_02_PlatformRequired()
    {
        var headers = new Dictionary<string, string>
        {
            { "X-Timezone", "America/Guayaquil" }
        };

        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.SystemError));
            Assert.That(response.Content, Does.Contain("La plataforma es invalida (X-Platform"));
        });
    }

    [Test]
    public async Task TCC_03_TimeRequired()
    {
        var headers = new Dictionary<string, string>
        {
            { "X-Timezone", "America/Guayaquil" },
            { "X-Platform", "Web" }
        };
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.SystemError));
            Assert.That(response.Content, Does.Contain("El timestamp tiene un formato invalido (X-Time"));
        });
    }

    [Test]
    public async Task TCC_04_MiddlewareSuccessful()
    {
        var headers = ConfigureContextCompleteHeaders();
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}

