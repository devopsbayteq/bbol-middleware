using BankCore.Integration.Interfaces;
using Common.WebApi.Clock;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic.Authentication;
using LogicApi.Model.Request.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using System.Linq.Expressions;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.Authentication;

public class LoginTests : BaseTests
{
    protected LoginHandler Handler;
    protected AppSetting CoreSettings;
    protected Mock<IClock> Clock;
    protected Mock<IUnitOfWork> UnitOfWork;
    protected Mock<IBankCoreServices> Services;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<LoginHandler>> logger = new();
        Clock = new();
        UnitOfWork = new();
        Services = new();
        CoreSettings = new AppSetting
        {
            RsaSecurity = RsaSecuritySettings,
            LoginSecurity = new()
            {
                DummyPassword = "Password@1"
            },
            AesSecurity = new()
            {
                Key = Guid.NewGuid().ToString()[..16]
            },
            Jwt = Jwt
        };

        Handler = new LoginHandler(logger.Object, Options.Create(CoreSettings), Clock.Object, UnitOfWork.Object, Services.Object);
    }

    [Test]
    public async Task TL_01_UserNotFound()
    {
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Is.EqualTo("Usuario no existe."));
        });
    }

    [Test]
    public async Task TL_02_UserBloqued()
    {
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                LockDate = DateTime.Now
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.AccountLocked));
            Assert.That(ex.Message, Does.Contain("Su cuenta se encuentra bloqueada en la fecha"));
        });
    }

    [Test]
    public async Task TL_03_ErrorPasswordConfiguration()
    {
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock
        };

        CoreSettings.LoginSecurity.DummyPassword = null;

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.SystemError));
            Assert.That(ex.Message, Does.Contain("La configuración de seguridad de login no está definida"));
        });
    }

    [Test]
    public async Task TL_04_PasswordMismatch()
    {
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock
        };

        CoreSettings.LoginSecurity.DummyPassword = Guid.NewGuid().ToString();

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("Contraseña incorrecta."));
        });
    }

    [Test]
    public async Task TL_05_DeviceIdError()
    {
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("No se pudo obtener el DeviceId del contexto."));
        });
    }

    [Test]
    public async Task TL_06_LoginSuccessful()
    {
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = Guid.NewGuid(),
                UserName = "UserName",
                FirstName = "FirstName"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.AddAsync(It.IsAny<Device>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                DeviceId = Guid.NewGuid().ToString()
            }));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
        });
    }

}
