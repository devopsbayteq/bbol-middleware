using ApiCore.Models;
using LogicApi.Model.Request.ContractBalance;
using LogicApi.Model.Response.ContractBalance;
using Moq;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Controllers;

/// <summary>
/// Pruebas unitarias para Controlador ContractBalance
/// </summary>
public partial class ControllerTests
{
    [Test]
    public async Task TCB_01_ContractsSuccessful()
    {
        var contractBalanceResponse = new GetContractBalanceResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GetContractBalanceRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(contractBalanceResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GetContractBalanceResponse>>(HttpMethod.Get, Settings.ContractBalanceUrl, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TCB_02_ContractsHomeSuccessful()
    {
        var homeDashboardResponse = new GetHomeDashboardResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GetHomeDashboardRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(homeDashboardResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GetHomeDashboardResponse>>(HttpMethod.Get, Settings.ContractBalanceHomeUrl, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}
