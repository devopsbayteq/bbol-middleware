using ApiCore.Models;
using Common.WebApi.Messages;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Middleware;

/// <summary>
/// Pruebas unitarias para Middleware ValidateIntegrity
/// </summary>
public partial class MiddlewareTest
{

 
    [Test]
    public async Task TVI_03_DecryptError()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers.Add("X-Content", Guid.NewGuid().ToString());
        headers.Add("X-Secret", Guid.NewGuid().ToString());
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers, addIntegrity: false);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.SystemError));
            Assert.That(response.Content, Does.Contain("Error al validar la integridad de datos"));
        });
    }


    [Test]
    public async Task TVI_05_MiddlewareSuccesful()
    {
        var headers = ValidateVersionCompleteHeaders();
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}
