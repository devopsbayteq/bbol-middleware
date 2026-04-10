using ApiCore.Models;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.Transaction;
using Moq;
using System.Net;

namespace BolivarianoBank.ApiCore.Tests.Controllers;

/// <summary>
/// Pruebas unitarias para Controlador Transaction
/// </summary>
public partial class ControllerTests
{
    [Test]
    public async Task TT_01_RecentSuccessful()
    {
        var getRecentTransactionsResponse = new GetRecentTransactionsResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GetRecentTransactionsRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(getRecentTransactionsResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GetRecentTransactionsResponse>>(HttpMethod.Get, Settings.TransactionUrl + "/recent", body: null, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TT_02_TransferSuccessful()
    {
        var bodyRequest = new CreateTransferRequest
        {
            Amount = 1,
            BeneficiaryContactGuid = Guid.NewGuid(),
            AccountGuid = Guid.NewGuid()
        };

        var createTransferResponse = new CreateTransferResponse();

        Mediator.Setup(m => m.Send(It.IsAny<CreateTransferRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(createTransferResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<CreateTransferResponse>>(HttpMethod.Post, Settings.TransactionUrl + "/transfer", body: bodyRequest, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }

    [Test]
    public async Task TT_03_Successful()
    {
        var getTransactionsQueryResponse = new GetTransactionsQueryResponse();

        Mediator.Setup(m => m.Send(It.IsAny<GetTransactionsQueryRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(getTransactionsQueryResponse));

        (var statusCode, var response) = await SendAsync<GenericResponse<GetTransactionsQueryResponse>>(HttpMethod.Get, Settings.TransactionUrl + $"?accountGuid={Guid.NewGuid()}", body: null, tokenRequired: true);

        Assert.Multiple(() =>
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.Not.Null);
        });
    }
}
