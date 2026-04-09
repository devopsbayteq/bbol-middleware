using ApiCore.Models;
using Common.WebApi.Messages;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Middleware;

/// <summary>
/// Pruebas unitarias para Middleware ValidateVersion
/// </summary>
public partial class MiddlewareTest
{
    [Test]
    public async Task TVV_01_VersionRequired()
    {
        var headers = ConfigureContextCompleteHeaders();
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.SystemError));
            Assert.That(response.Content, Does.Contain("No se puede parsear la versión del móvil"));
        });
    }

    [Test]
    public async Task TVV_02_VersionInvalid()
    {
        var headers = ConfigureContextCompleteHeaders();
        headers.Add("X-Version", "1.0.0.0");
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.VersionNotAllowed));
            Assert.That(response.Content, Does.Contain("La versión mínima soportada es"));
        });
    }

    [Test]
    public async Task TVV_03_MiddlewareSuccessful()
    {
        var headers = ValidateVersionCompleteHeaders();
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}
