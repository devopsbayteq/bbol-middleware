using ApiCore.Models;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Response.Authentication;
using LogicApi.Model.Response.Beneficiary;
using Moq;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Controllers;

/// <summary>
/// Pruebas unitarias para Controlador Beneficiary
/// </summary>
public partial class ControllerTests
{
    [Test]
    public async Task TB_01_ContactsUnauthorized()
    {
        var beneficiaryContactsResponse = new GetBeneficiaryContactsResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GetBeneficiaryContactsRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(beneficiaryContactsResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<LoginResponse>>(HttpMethod.Get, Settings.ContactsUrl);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }

    [Test]
    public async Task TB_02_ContactsSuccessful()
    {
        var beneficiaryContactsResponse = new GetBeneficiaryContactsResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GetBeneficiaryContactsRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(beneficiaryContactsResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<LoginResponse>>(HttpMethod.Get, Settings.ContactsUrl, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}
