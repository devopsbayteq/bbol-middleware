using ApiCore.Models;
using Common.WebApi.Messages;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Middleware;

public partial class MiddlewareTest
{
    [Test]
    public async Task TVD_01_MiddlewareSuccessful()
    {
        var headers = ValidateRootCompleteHeaders();
        headers["X-Fingerprint"] = Settings.FingerprintInvalidStatus;

        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.DeviceStateNotAllowed));
            Assert.That(response.Content, Does.Contain("El estado de la actividad del dispositivo es inválido"));
        });
    }
}
