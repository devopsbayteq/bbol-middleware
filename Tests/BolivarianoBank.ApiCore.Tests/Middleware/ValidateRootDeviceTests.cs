using ApiCore.Models;
using Common.WebApi.Messages;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Middleware;

/// <summary>
/// Pruebas unitarias para Middleware ValidateRoot
/// </summary>
public partial class MiddlewareTest
{
    [Test]
    public async Task TVR_01_FingerprintRequired()
    {
        var headers = ValidateVersionCompleteHeaders();
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.HeaderNotFound));
            Assert.That(response.Content, Does.Contain("El header X-Fingerprint es requerido"));
        });
    }

    [Test]
    public async Task TVR_02_IsRoot()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers.Add("X-Fingerprint", Settings.FingerprintRootMock);
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.DeviceRootNotAllowed));
            Assert.That(response.Content, Does.Contain("El dispositivo se encuentra en modo Root"));
        });
    }

    [Test]
    public async Task TVR_03_IsDebugger()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers.Add("X-Fingerprint", Settings.FingerprintDebuggerMock);
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.DeviceDebuggerNotAllowed));
            Assert.That(response.Content, Does.Contain("El dispositivo se encuentra en modo Debugger"));
        });
    }

    [Test]
    public async Task TVR_04_IsDevelopment()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers.Add("X-Fingerprint", Settings.FingerprintDevelopmentMock);
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.DeviceDevelopmentNotAllowed));
            Assert.That(response.Content, Does.Contain("El dispositivo se encuentra en modo Development"));
        });
    }

    [Test]
    public async Task TVR_05_IsPhysicalDevice()
    {
        var headers = ValidateVersionCompleteHeaders();
        headers.Add("X-Fingerprint", Settings.FingerprintNoPhysicalMock);
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Code, Is.EqualTo((int)MessageCodes.EmulatorDeviceNotAllowed));
            Assert.That(response.Content, Does.Contain("El dispositivo está en modo Emulador"));
        });
    }

    [Test]
    public async Task TVR_06_MiddlewareSuccessful()
    {
        var headers = ValidateRootCompleteHeaders();
        
        (var statusCode, var response) = await SendAsync<GenericResponse<string>>(HttpMethod.Post, Settings.LoginUrl, body: new { }, headers: headers);


        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

}
