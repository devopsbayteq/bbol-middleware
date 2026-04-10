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
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        
        var request = new LoginRequest()
        {
            Username = Settings.UserMock,
            Password = Settings.PasswordMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId",
                    Model = "TestModel",
                    Brand = "TestBrand"
                },
                RequestId = Guid.NewGuid().ToString("N")
            }
        };

        // Setup Session settings
        CoreSettings.Session = new()
        {
            SessionTimeSeconds = 1800,
            InactivityTimeoutSeconds = 300
        };

        // Mock user retrieval
        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "UserName",
                FirstName = "FirstName",
                Surname = "LastName",
                DocumentNumber = "1234567890",
                Alias = "TestAlias",
                FailedLoginAttempts = 0
            }));

        // Mock user update (reset failed attempts)
        UnitOfWork.Setup(u => u.UserRepository.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Mock BankCore ValidateUser
        Services.Setup(s => s.ValidateUserAsync(It.IsAny<BankCore.Integration.Models.User.ValidateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BankCore.Integration.Models.User.ValidateUserResponse());

        // Mock BankCore ValidateUserPassword
        Services.Setup(s => s.ValidateUserPasswordAsync(It.IsAny<BankCore.Integration.Models.User.ValidateUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BankCore.Integration.Models.User.ValidateUserPasswordResponse());

        // Mock device retrieval (device doesn't exist yet)
        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(null!));

        // Mock device creation
        UnitOfWork.Setup(u => u.DeviceRepository.AddAsync(It.IsAny<Device>()))
            .Returns(Task.FromResult( new Device
            {
                Guid = deviceGuid,
                UserGuid = Guid.NewGuid(),
                DeviceId = "",
                Model = "",
                Brand = "",
                PlatformType = 0,
                RegisterDate = DateTime.Now
            }));

        // Mock Clock for device registration
        Clock.Setup(c => c.Now()).Returns(DateTime.Now);

        // Mock AccountUserRepository for EnsureOwnAccountsAsBeneficiariesAsync
        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<PersistenceDb.Models.Core.AccountUser, bool>>>()))
            .ReturnsAsync(new List<PersistenceDb.Models.Core.AccountUser>());

        // Mock BeneficiaryRepository for EnsureOwnAccountsAsBeneficiariesAsync
        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByAsync(It.IsAny<Expression<Func<PersistenceDb.Models.Core.Beneficiary, bool>>>()))
            .ReturnsAsync(new List<PersistenceDb.Models.Core.Beneficiary>());

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(response.FirstName, Is.EqualTo("FirstName"));
            Assert.That(response.Alias, Is.EqualTo("TestAlias"));
            Assert.That(response.SessionTimeSeconds, Is.EqualTo(1800));
            Assert.That(response.InactivityTimeoutSeconds, Is.EqualTo(300));
        });
    }

}
