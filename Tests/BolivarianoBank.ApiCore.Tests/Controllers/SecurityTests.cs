using ApiCore.Models;
using LogicApi.Model.Request.Authorization;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response;
using LogicApi.Model.Response.Authorization;
using LogicApi.Model.Response.Security;
using Moq;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Controllers;

/// <summary>
/// Pruebas unitarias para Controlador Security
/// </summary>
public partial class ControllerTests
{
    [Test]
    public async Task TS_01_PublicKeySuccessful()
    {
        var publicKeyResponse = new GetPublicKeyResponse(Guid.NewGuid().ToString());

        Mediator.Setup(m => m.Send(It.IsAny<GetPublicKeyRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(publicKeyResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GetPublicKeyResponse>>(HttpMethod.Get, Settings.SecurityUrl + "/public-key", body: null);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TS_02_CertificateSuccessful()
    {
        var bodyRequest = new GetCertificateRequest
        {
            SecretEncryptBase64 = Guid.NewGuid().ToString(),
            SecretEncryptSignBase64 = Guid.NewGuid().ToString(),
            SecretIvEncryptBase64 = Guid.NewGuid().ToString()
        };

        var certificateResponse = new GetCertificateResponse(new("", ""), true);

        Mediator.Setup(m => m.Send(It.IsAny<GetCertificateRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(certificateResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GetCertificateResponse>>(HttpMethod.Post, Settings.SecurityUrl + "/certificate", body: bodyRequest);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TS_03_ValidateOtpSuccessful()
    {
        var bodyRequest = new ValidateOtpRequest
        {
            Otp = Guid.NewGuid().ToString()
        };

        var genericCommonOperationResponse = new GenericCommonOperationResponse(string.Empty);

        Mediator.Setup(m => m.Send(It.IsAny<ValidateOtpRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(genericCommonOperationResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GenericCommonOperationResponse>>(HttpMethod.Post, Settings.SecurityUrl + "/validate-otp", body: bodyRequest);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TS_04_RegisterBiometricSuccessful()
    {
        var bodyRequest = new RegisterBiometricRequest
        {
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Guid.NewGuid().ToString(),
            MobilePublicKeyBase64 = Guid.NewGuid().ToString()
        };

        var genericCommonOperationResponse = new GenericCommonOperationResponse(string.Empty);

        Mediator.Setup(m => m.Send(It.IsAny<RegisterBiometricRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(genericCommonOperationResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GenericCommonOperationResponse>>(HttpMethod.Post, Settings.SecurityUrl + "/biometric-registration", body: bodyRequest, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TS_05_ValidateTransacctionSuccessful()
    {
        var bodyRequest = new ValidateTransactionAmountRequest
        {
            Amount =1,
            BeneficiaryContactGuid = Guid.NewGuid().ToString(),
            AccountGuid = Guid.NewGuid().ToString()
        };

        var validateTransactionAmountResponse = new ValidateTransactionAmountResponse();

        Mediator.Setup(m => m.Send(It.IsAny<ValidateTransactionAmountRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validateTransactionAmountResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<ValidateTransactionAmountResponse>>(HttpMethod.Post, Settings.SecurityUrl + "/validate-transaction-amount", body: bodyRequest, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TS_06_BiometricChallengeSuccessful()
    {
        var bodyRequest = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Guid.NewGuid().ToString()
        };

        var generateBiometricChallengeResponse = new GenerateBiometricChallengeResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GenerateBiometricChallengeRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(generateBiometricChallengeResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GenerateBiometricChallengeResponse>>(HttpMethod.Post, Settings.SecurityUrl + "/biometric-challenge", body: bodyRequest);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TS_07_RsaEncryptSuccessful()
    {
        var bodyRequest = new RsaEncryptTextRequest
        {
            PlainText = Guid.NewGuid().ToString()
        };

        var rsaEncryptTextResponse = new RsaEncryptTextResponse();

        Mediator.Setup(m => m.Send(It.IsAny<RsaEncryptTextRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(rsaEncryptTextResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<RsaEncryptTextResponse>>(HttpMethod.Post, Settings.SecurityUrl + "/rsa/encrypt", body: bodyRequest);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}

