using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic.Security;
using LogicApi.Model.Request.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using System.Linq.Expressions;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.Security;

/// <summary>
/// Pruebas unitarias para GenerateBiometricChallengeHandler
/// </summary>
public class GenerateBiometricChallengeTests : BaseTests
{
    protected GenerateBiometricChallengeHandler Handler;
    protected AppSetting CoreSettings;
    protected Mock<IUnitOfWork> UnitOfWork;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<GenerateBiometricChallengeHandler>> logger = new();
        UnitOfWork = new();
        CoreSettings = new AppSetting
        {
            RsaSecurity = RsaSecuritySettings
        };

        Handler = new GenerateBiometricChallengeHandler(
            logger.Object,
            Options.Create(CoreSettings),
            UnitOfWork.Object);
    }

    [Test]
    public async Task TGBC_01_UserEncryptedInvalid()
    {
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = ""
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("No se pudo validar el usuario encriptado"));
        });
    }

    [Test]
    public async Task TGBC_02_UserNotFound()
    {
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock
        };

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
    public async Task TGBC_03_DeviceIdNotInContext()
    {
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = Guid.NewGuid(),
                UserName = "TestUser"
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("No se pudo obtener el DeviceGuid del contexto"));
        });
    }

    [Test]
    public async Task TGBC_04_DeviceNotRegistered()
    {
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = Guid.NewGuid(),
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.InvalidCredentials));
            Assert.That(ex.Message, Does.Contain("No existe dispositivo registrado para el usuario"));
        });
    }

    [Test]
    public async Task TGBC_05_GenerateChallengeForNewUserDevice()
    {
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        // No existe challenge previo
        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(null!));

        var newChallenge = new UserDeviceChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            DeviceId = deviceGuid,
            BiometricChallenge = string.Empty
        };

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.AddAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(newChallenge));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(newChallenge));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Challenge, Is.Not.Null);
            Assert.That(response.Challenge, Is.Not.Empty);
            Assert.That(response.Challenge, Is.Not.Empty);
        });

        // Verify that AddAsync was called to create new challenge
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.AddAsync(It.Is<UserDeviceChallenge>(
            c => c.UserId == userGuid && c.DeviceId == deviceGuid
        )), Times.Once);

        // Verify that UpdateAsync was called to update the challenge
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()), Times.Once);
    }

    [Test]
    public async Task TGBC_06_UpdateExistingChallenge()
    {
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        var oldChallenge = "OldChallengeValue123";
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        var existingChallenge = new UserDeviceChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            DeviceId = deviceGuid,
            BiometricChallenge = oldChallenge
        };

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(existingChallenge));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(existingChallenge));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Challenge, Is.Not.Null);
            Assert.That(response.Challenge, Is.Not.Empty);
            Assert.That(response.Challenge, Is.Not.EqualTo(oldChallenge));
        });

        // Verify that AddAsync was NOT called (challenge already exists)
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.AddAsync(It.IsAny<UserDeviceChallenge>()), Times.Never);

        // Verify that UpdateAsync was called to update the challenge
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.UpdateAsync(It.Is<UserDeviceChallenge>(
            c => c.UserId == userGuid && c.DeviceId == deviceGuid && c.BiometricChallenge != oldChallenge
        )), Times.Once);
    }

    [Test]
    public async Task TGBC_07_ChallengeIsBase64Encoded()
    {
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        var existingChallenge = new UserDeviceChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            DeviceId = deviceGuid,
            BiometricChallenge = "OldChallenge"
        };

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(existingChallenge));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(existingChallenge));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Challenge, Is.Not.Null);
            
            // Verify it's valid Base64
            try
            {
                var bytes = Convert.FromBase64String(response.Challenge);
                Assert.That(bytes, Is.Not.Null);
                Assert.That(bytes, Is.Not.Empty);
            }
            catch (FormatException)
            {
                Assert.Fail("Challenge is not valid Base64");
            }
        });
    }

    [Test]
    public async Task TGBC_08_MultipleChallengesAreDifferent()
    {
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        var existingChallenge = new UserDeviceChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            DeviceId = deviceGuid,
            BiometricChallenge = "OldChallenge"
        };

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(existingChallenge));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(existingChallenge));

        // Generate first challenge
        var response1 = await Handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Generate second challenge
        var response2 = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response1.Challenge, Is.Not.Null);
            Assert.That(response2.Challenge, Is.Not.Null);
            Assert.That(response1.Challenge, Is.Not.EqualTo(response2.Challenge));
        });
    }

    [Test]
    public async Task TGBC_09_ChallengeHasCorrectLength()
    {
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        var existingChallenge = new UserDeviceChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            DeviceId = deviceGuid,
            BiometricChallenge = "OldChallenge"
        };

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(existingChallenge));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(existingChallenge));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Challenge, Is.Not.Null);
            var bytes = Convert.FromBase64String(response.Challenge);
            Assert.That(bytes, Has.Length.EqualTo(32));
        });
    }

    [Test]
    public async Task TGBC_10_VerifyRepositoryCallsForNewChallenge()
    {
        var userGuid = Guid.NewGuid();
        var deviceGuid = Guid.NewGuid();
        var request = new GenerateBiometricChallengeRequest
        {
            UserEncryptBase64 = Settings.UserMock,
            ContextRequest = new()
            {
                Headers = new()
                {
                    DeviceId = "DeviceId123"
                }
            }
        };

        UnitOfWork.Setup(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(Task.FromResult<User>(new User
            {
                Guid = userGuid,
                UserName = "TestUser"
            }));

        UnitOfWork.Setup(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .Returns(Task.FromResult<Device>(new Device
            {
                Guid = deviceGuid,
                DeviceId = "DeviceId123"
            }));

        // No existing challenge
        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()))
            .Returns(Task.FromResult<UserDeviceChallenge>(null!));

        var newChallenge = new UserDeviceChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userGuid,
            DeviceId = deviceGuid,
            BiometricChallenge = string.Empty
        };

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.AddAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(newChallenge));

        UnitOfWork.Setup(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()))
            .Returns(Task.FromResult(newChallenge));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(response, Is.Not.Null);

        // Verify all repository calls
        UnitOfWork.Verify(u => u.UserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        UnitOfWork.Verify(u => u.DeviceRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()), Times.Once);
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<UserDeviceChallenge, bool>>>()), Times.Once);
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.AddAsync(It.IsAny<UserDeviceChallenge>()), Times.Once);
        UnitOfWork.Verify(u => u.UserDeviceChallengeRepository.UpdateAsync(It.IsAny<UserDeviceChallenge>()), Times.Once);
    }
}
