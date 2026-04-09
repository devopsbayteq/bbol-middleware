using ApiCore.Models;
using Common.WebApi.Messages;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Middleware;

/// <summary>
/// Pruebas unitarias para Middleware ValidateClient
/// </summary>
public partial class MiddlewareTest
{
    [Test]
    public async Task TVC_01_TimeRequired()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers["X-Time"] = (DateTimeOffset.Now.AddHours(1)).ToUnixTimeSeconds().ToString();

        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.ClientTimeOutdated));
            Assert.That(response.Content, Does.Contain("El header X-Time"));
        });
    }

    [Test]
    public async Task TVC_02_MiddlewareSuccessful()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers["X-Time"] = (DateTimeOffset.Now.AddSeconds(1)).ToUnixTimeSeconds().ToString();

        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}
