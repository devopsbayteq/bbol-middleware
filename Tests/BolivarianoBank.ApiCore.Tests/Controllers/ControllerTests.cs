using MediatR;
using Moq;

namespace BolivarianoBank.ApiCore.Tests.Controllers;

public partial class ControllerTests : BaseTests
{
    protected Mock<IMediator> Mediator;

    [SetUp]
    public async Task SetUp()
    {
        Mediator = new();
        Server = await ApiCoreWebApplicationFactory.CreateServer(Mediator);
        Client = Server.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client?.Dispose();
        Server?.Dispose();
    }
}