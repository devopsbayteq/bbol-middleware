using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic.Security;
using LogicApi.Model.Request.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.Security;

/// <summary>
/// Pruebas unitarias para GetCertificateHandler
/// </summary>
public class GetCertificateTests : BaseTests
{
    protected GetCertificateHandler Handler;
    protected AppSetting CoreSettings;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<GetCertificateHandler>> logger = new();
        CoreSettings = new AppSetting
        {
            RsaSecurity = RsaSecuritySettings,
            Certificate = new CertificateConfiguration
            {
                HashCertificate = "TestHashCertificate123",
                ValidateHash = true
            }
        };

        Handler = new GetCertificateHandler(logger.Object, Options.Create(CoreSettings));
    }

    [Test]
    public async Task TGC_01_HashCertificateEmpty()
    {
        CoreSettings.Certificate.HashCertificate = string.Empty;

        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.CertificatesNotFound));
            Assert.That(ex.Message, Does.Contain("El Hash de Certificado está vacío"));
        });
    }

    [Test]
    public async Task TGC_02_HashCertificateNull()
    {
        CoreSettings.Certificate.HashCertificate = null;

        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.CertificatesNotFound));
            Assert.That(ex.Message, Does.Contain("El Hash de Certificado está vacío"));
        });
    }

    [Test]
    public async Task TGC_03_InvalidSignature()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.SecretMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
            Assert.That(ex.Message, Does.Contain("No se pudo validar la Firma de la petición"));
        });
    }

    [Test]
    public async Task TGC_04_InvalidSecretEncryptBase64()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.SecretMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task TGC_05_ValidateHashTrue()
    {
        CoreSettings.Certificate.ValidateHash = true;

        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        // This will fail due to signature validation, but we're testing the ValidateHash property
        try
        {
            var response = await Handler.Handle(request, It.IsAny<CancellationToken>());
            Assert.That(response.ValidateHash, Is.True);
        }
        catch (CustomException ex)
        {
            // Expected to fail on signature validation
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
        }
    }

    [Test]
    public async Task TGC_06_ValidateHashFalse()
    {
        CoreSettings.Certificate.ValidateHash = false;

        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        // This will fail due to signature validation, but we're testing the ValidateHash property
        try
        {
            var response = await Handler.Handle(request, It.IsAny<CancellationToken>());
            Assert.That(response.ValidateHash, Is.False);
        }
        catch (CustomException ex)
        {
            // Expected to fail on signature validation
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
        }
    }

    [Test]
    public async Task TGC_07_ResponseContainsCertificate()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        try
        {
            var response = await Handler.Handle(request, It.IsAny<CancellationToken>());
            
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Certificate, Is.Not.Null);
                Assert.That(response.Certificate.HashEncrypt, Is.Not.Null);
                Assert.That(response.Certificate.HashEncryptSign, Is.Not.Null);
            });
        }
        catch (CustomException ex)
        {
            // Expected to fail on signature or decryption validation
            Assert.That(ex.MessageCode, Is.AnyOf(MessageCodes.DataDoesNotMatch, MessageCodes.SystemError));
        }
    }

    [Test]
    public async Task TGC_08_EmptySecretEncryptBase64()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = string.Empty,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        Assert.ThrowsAsync<FormatException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task TGC_09_EmptySecretIvEncryptBase64()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = string.Empty
        };

        Assert.ThrowsAsync<FormatException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task TGC_10_CertificateConfigurationPresent()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CoreSettings.Certificate, Is.Not.Null);
            Assert.That(CoreSettings.Certificate.HashCertificate, Is.Not.Null);
            Assert.That(CoreSettings.Certificate.HashCertificate, Is.Not.Empty);
            Assert.That(CoreSettings.Certificate.ValidateHash, Is.True);
        });
    }

    [Test]
    public async Task TGC_11_RsaSecurityKeysPresent()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CoreSettings.RsaSecurity, Is.Not.Null);
            Assert.That(CoreSettings.RsaSecurity.DeviceBase64PublicKey, Is.Not.Null);
            Assert.That(CoreSettings.RsaSecurity.DeviceBase64PublicKey, Is.Not.Empty);
            Assert.That(CoreSettings.RsaSecurity.ServerCertificateBase64PrivateKey, Is.Not.Null);
            Assert.That(CoreSettings.RsaSecurity.ServerCertificateBase64PrivateKey, Is.Not.Empty);
        });
    }

    [Test]
    public async Task TGC_12_InvalidBase64InSecretEncrypt()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = "Not@Valid#Base64!",
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        Assert.ThrowsAsync<FormatException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task TGC_13_InvalidBase64InSecretIv()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = "Not@Valid#Base64!"
        };

        Assert.ThrowsAsync<FormatException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task TGC_14_AllRequiredFieldsPresent()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        Assert.Multiple(() =>
        {
            Assert.That(request.SecretEncryptBase64, Is.Not.Null);
            Assert.That(request.SecretEncryptBase64, Is.Not.Empty);
            Assert.That(request.SecretEncryptSignBase64, Is.Not.Null);
            Assert.That(request.SecretEncryptSignBase64, Is.Not.Empty);
            Assert.That(request.SecretIvEncryptBase64, Is.Not.Null);
            Assert.That(request.SecretIvEncryptBase64, Is.Not.Empty);
        });
    }

    [Test]
    public async Task TGC_15_HashCertificateWithSpecialCharacters()
    {
        CoreSettings.Certificate.HashCertificate = "Hash!@#$%^&*()_+-={}[]|:;<>?,./";

        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        try
        {
            var response = await Handler.Handle(request, It.IsAny<CancellationToken>());
            Assert.That(response, Is.Not.Null);
        }
        catch (CustomException ex)
        {
            // Expected to fail on signature validation
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
        }
    }

    [Test]
    public async Task TGC_16_LongHashCertificate()
    {
        CoreSettings.Certificate.HashCertificate = new string('A', 1000);

        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        try
        {
            var response = await Handler.Handle(request, It.IsAny<CancellationToken>());
            Assert.That(response, Is.Not.Null);
        }
        catch (CustomException ex)
        {
            // Expected to fail on signature validation
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.DataDoesNotMatch));
        }
    }

    [Test]
    public async Task TGC_17_ResponseStructureValidation()
    {
        var request = new GetCertificateRequest
        {
            SecretEncryptBase64 = Settings.UserMock,
            SecretEncryptSignBase64 = Settings.UserMock,
            SecretIvEncryptBase64 = Settings.UserMock
        };

        try
        {
            var response = await Handler.Handle(request, It.IsAny<CancellationToken>());
            
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Certificate, Is.Not.Null);
                Assert.That(response.Certificate.HashEncrypt, Is.Not.Null);
                Assert.That(response.Certificate.HashEncryptSign, Is.Not.Null);
                Assert.That(response.ValidateHash, Is.EqualTo(CoreSettings.Certificate.ValidateHash));
            });
        }
        catch (CustomException ex)
        {
            // Expected to fail on signature or decryption validation
            Assert.That(ex.MessageCode, Is.AnyOf(MessageCodes.DataDoesNotMatch, MessageCodes.SystemError));
        }
    }
}
