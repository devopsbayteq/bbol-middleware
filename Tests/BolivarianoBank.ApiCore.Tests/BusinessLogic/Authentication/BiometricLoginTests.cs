using BankCore.Integration.Interfaces;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic.Authentication;
using LogicApi.Model.Request.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using System.Linq.Expressions;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.Authentication;

/// <summary>
/// Pruebas unitarias para BiometricLoginHandler
/// </summary>
public class BiometricLoginTests : BaseTests
{
    protected BiometricLoginHandler Handler;
    protected AppSetting CoreSettings;
    protected Mock<IUnitOfWork> UnitOfWork;
    protected Mock<IBankCoreServices> BankCoreServices;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<BiometricLoginHandler>> logger = new();
        UnitOfWork = new();
        BankCoreServices = new();
        CoreSettings = new AppSetting
        {
            RsaSecurity = RsaSecuritySettings,
            AesSecurity = new()
            {
                Key = Guid.NewGuid().ToString()[..16]
            },
            Jwt = Jwt
        };

        Handler = new BiometricLoginHandler(logger.Object, Options.Create(CoreSettings), UnitOfWork.Object, BankCoreServices.Object);
    }

    [Test]
    public async Task TBL_01_UsernameEncryptedInvalid()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = "",
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Settings.UserMock
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("No se pudo validar el username encriptado"));
        });
    }

    [Test]
    public async Task TBL_02_DeviceIdNotInContext()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Settings.UserMock
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("No se pudo obtener el DeviceGuid del contexto"));
        });
    }

    [Test]
    public async Task TBL_03_DeviceNotRegistered()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("El dispositivo no está registrado para el usuario"));
        });
    }

    [Test]
    public async Task TBL_04_UserNotFound()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = Guid.NewGuid(),
                DeviceId = "DeviceId123"
            }));

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("El usuario no existe"));
        });
    }

    [Test]
    public async Task TBL_05_UserBlocked()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = Guid.NewGuid(),
                DeviceId = "DeviceId123"
            }));

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = Guid.NewGuid(),
                UserName = "TestUser",
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
    public async Task TBL_06_ChallengeNotFound()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = Guid.NewGuid().ToString(),
            ChallengeSignBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        var deviceGuid = Guid.NewGuid();
        var userGuid = Guid.NewGuid();

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
            Assert.That(ex.Message, Does.Contain("El challenge no coincide con el generado"));
        });
    }

    [Test]
    public async Task TBL_07_ChallengeMismatch()
    {
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = "DifferentChallenge",
            ChallengeSignBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        var deviceGuid = Guid.NewGuid();
        var userGuid = Guid.NewGuid();

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(new UserDeviceChallenge
            {
                UserId = userGuid,
                DeviceId = deviceGuid,
                BiometricChallenge = "ExpectedChallenge"
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
            Assert.That(ex.Message, Does.Contain("El challenge no coincide con el generado"));
        });
    }

    [Test]
    public async Task TBL_08_BiometricPublicKeyNotRegistered()
    {
        var challenge = Guid.NewGuid().ToString();
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = challenge,
            ChallengeSignBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        var deviceGuid = Guid.NewGuid();
        var userGuid = Guid.NewGuid();

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123",
                BiometricPublicKey = null
            }));

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(new UserDeviceChallenge
            {
                UserId = userGuid,
                DeviceId = deviceGuid,
                BiometricChallenge = challenge
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
            Assert.That(ex.Message, Does.Contain("No existe llave pública biométrica registrada para el usuario"));
        });
    }

    [Test]
    public async Task TBL_09_InvalidBiometricSignature()
    {
        var challenge = Guid.NewGuid().ToString();
        var request = new BiometricLoginRequest()
        {
            UsernameEncryptBase64 = Settings.UserMock,
            Challenge = challenge,
            ChallengeSignBase64 = "InvalidSignature",
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        var deviceGuid = Guid.NewGuid();
        var userGuid = Guid.NewGuid();

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123",
                BiometricPublicKey = RsaSecuritySettings.ServerBase64PublicKey
            }));

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(new UserDeviceChallenge
            {
                UserId = userGuid,
                DeviceId = deviceGuid,
                BiometricChallenge = challenge
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
            Assert.That(ex.Message, Does.Contain("La firma biométrica del challenge es inválida"));
        });
    }    
}
