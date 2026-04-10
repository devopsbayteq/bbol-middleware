using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Transfer;
using BankCore.Integration.Models.User;
using BankCore.Integration.Services;

namespace BolivarianoBank.ApiCore.Tests.Integration.Services;

/// <summary>
/// Pruebas unitarias para MockBankCoreServices
/// </summary>
public class MockBankCoreServicesTests : BaseTests
{
    protected IBankCoreServices Service;

    [SetUp]
    public void Setup()
    {
        Service = new MockBankCoreServices();
    }

    [Test]
    public async Task TMBCS_01_GetAccessTokenAsync_ReturnsMockToken()
    {
        var token = await Service.GetAccessTokenAsync();

        Assert.Multiple(() =>
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.EqualTo("mock-access-token"));
        });
    }

    [Test]
    public async Task TMBCS_02_GetAccessTokenAsync_WithCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = await Service.GetAccessTokenAsync(cts.Token);

        Assert.That(token, Is.EqualTo("mock-access-token"));
    }

    [Test]
    public async Task TMBCS_03_ValidateUserAsync_WithValidRequest()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Codigo, Is.EqualTo("0"));
            Assert.That(response.Mensaje, Does.Contain("exitosa"));
            Assert.That(response.Resultado, Is.Not.Null);
        });
    }

    [Test]
    public async Task TMBCS_04_ValidateUserAsync_UsesProvidedSessionId()
    {
        var sessionId = "custom-session-123";
        var request = new ValidateUserRequest(sessionId, "testuser", "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Resultado.Header.SessionId, Is.EqualTo(sessionId));
        });
    }

    [Test]
    public async Task TMBCS_05_ValidateUserAsync_UsesDefaultSessionIdWhenEmpty()
    {
        var request = new ValidateUserRequest("", "testuser", "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Resultado.Header.SessionId, Is.EqualTo("MOCK-SESSION"));
        });
    }

    [Test]
    public async Task TMBCS_06_ValidateUserAsync_ContainsCustomerData()
    {
        var customerName = "john.doe";
        var request = new ValidateUserRequest("session123", customerName, "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Resultado.Customer, Is.Not.Null);
            Assert.That(response.Resultado.Customer.FirstName1, Is.EqualTo("MOCK"));
            Assert.That(response.Resultado.Customer.LastName1, Is.EqualTo("USUARIO"));
            Assert.That(response.Resultado.Customer.Token, Is.Not.Null);
            Assert.That(response.Resultado.Customer.Token.Value, Does.Contain("MOCK-SYNTHETIC-CUSTOMER-TOKEN"));
        });
    }

    [Test]
    public async Task TMBCS_07_ValidateUserAsync_ContainsAuthenticationData()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Resultado.Authentication, Is.Not.Null);
            Assert.That(response.Resultado.Authentication.ActivationRequired, Is.EqualTo("false"));
            Assert.That(response.Resultado.Authentication.DynamicAuthenticacion, Is.Not.Null);
            Assert.That(response.Resultado.Authentication.DynamicAuthenticacion.Login, Is.EqualTo("true"));
        });
    }

    [Test]
    public async Task TMBCS_08_ValidateUserPasswordAsync_WithValidRequest()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Codigo, Is.EqualTo("0"));
            Assert.That(response.Mensaje, Does.Contain("exitosa"));
            Assert.That(response.Resultado, Is.Not.Null);
        });
    }

    [Test]
    public async Task TMBCS_09_ValidateUserPasswordAsync_UsesProvidedSessionId()
    {
        var sessionId = "custom-session-456";
        var request = new ValidateUserPasswordRequest
        {
            SessionId = sessionId,
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.That(response.Resultado.Header.SessionId, Is.EqualTo(sessionId));
    }

    [Test]
    public async Task TMBCS_10_ValidateUserPasswordAsync_UsesDefaultSessionIdWhenEmpty()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.That(response.Resultado.Header.SessionId, Is.EqualTo("MOCK-SESSION"));
    }

    [Test]
    public async Task TMBCS_11_ValidateUserPasswordAsync_ContainsGenericData()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.Generic, Is.Not.Null);
            Assert.That(response.Resultado.Generic.CanLogin, Is.EqualTo("true"));
            Assert.That(response.Resultado.Generic.Result, Is.EqualTo("true"));
            Assert.That(response.Resultado.Generic.IdentificationNumberMasked, Is.EqualTo("00******00"));
        });
    }

    [Test]
    public async Task TMBCS_12_ValidateUserPasswordAsync_ContainsCustomerData()
    {
        var alias = "john.doe";
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = alias,
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.Customer, Is.Not.Null);
            Assert.That(response.Resultado.Customer.FirstName1, Is.EqualTo("MOCK"));
            Assert.That(response.Resultado.Customer.LastName1, Is.EqualTo("USUARIO"));
            Assert.That(response.Resultado.Customer.Token, Is.Not.Null);
            Assert.That(response.Resultado.Customer.Token.Value, Does.Contain("MOCK-SYNTHETIC-PASSWORD-FLOW-TOKEN"));
        });
    }

    [Test]
    public async Task TMBCS_13_ValidateUserPasswordAsync_ContainsAuthenticationsList()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.Authentications, Is.Not.Null);
            Assert.That(response.Resultado.Authentications, Is.Not.Empty);
            Assert.That(response.Resultado.Customer.Authentications, Is.Not.Null);
            Assert.That(response.Resultado.Customer.Authentications, Is.Not.Empty);
        });
    }

    [Test]
    public async Task TMBCS_14_TransferBetweenAccountsAsync_WithValidRequest()
    {
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = DateTimeOffset.UtcNow,
            Tipo = "TRF",
            Auditoria = new TransferAuditInfo
            {
                Secuencial = "SEQ123456"
            }
        };

        var response = await Service.TransferBetweenAccountsAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SecuenciaTransaccion, Is.Not.Null);
            Assert.That(response.SecuenciaTransaccion, Is.EqualTo("SEQ123456"));
            Assert.That(response.FechaProceso, Is.Not.Null);
            Assert.That(response.FechaTransaccion, Is.Not.Null);
        });
    }

    [Test]
    public async Task TMBCS_15_TransferBetweenAccountsAsync_GeneratesSequenceWhenNotProvided()
    {
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = DateTimeOffset.UtcNow,
            Tipo = "TRF"
        };

        var response = await Service.TransferBetweenAccountsAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SecuenciaTransaccion, Is.Not.Null);
            Assert.That(response.SecuenciaTransaccion, Does.StartWith("MOCK"));
        });
    }

    [Test]
    public async Task TMBCS_16_TransferBetweenAccountsAsync_UsesProvidedDate()
    {
        var specificDate = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = specificDate,
            Tipo = "TRF"
        };

        var response = await Service.TransferBetweenAccountsAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.FechaProceso, Does.Contain("2024-06-15"));
            Assert.That(response.FechaTransaccion, Does.Contain("2024-06-15"));
        });
    }

    [Test]
    public async Task TMBCS_17_TransferBetweenAccountsAsync_WithCancellationToken()
    {
        var request = new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = DateTimeOffset.UtcNow,
            Tipo = "TRF"
        };

        using var cts = new CancellationTokenSource();
        var response = await Service.TransferBetweenAccountsAsync(request, cts.Token);

        Assert.That(response, Is.Not.Null);
    }

    [Test]
    public async Task TMBCS_18_ValidateUserAsync_ContainsElectronicsContact()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.ElectronicsContact, Is.Not.Null);
            Assert.That(response.Resultado.ElectronicsContact.EmailAddressComplete, Is.EqualTo("mock@bolivariano.com"));
        });
    }

    [Test]
    public async Task TMBCS_19_ValidateUserPasswordAsync_ContainsInstitutionData()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.Institution, Is.Not.Null);
            Assert.That(response.Resultado.Institution.CustomerName, Is.EqualTo("MOCK"));
            Assert.That(response.Resultado.Institution.Segment, Is.Not.Null);
            Assert.That(response.Resultado.Institution.Segment.Name, Is.EqualTo("SILVER-SILVER"));
        });
    }

    [Test]
    public async Task TMBCS_20_ValidateUserPasswordAsync_ContainsRoleTypes()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.RoleTypes, Is.Not.Null);
            Assert.That(response.Resultado.RoleTypes, Is.Not.Empty);
            Assert.That(response.Resultado.RoleTypes[0].ShortDesc, Is.EqualTo("OPERADOR_COMUN"));
        });
    }

    [Test]
    public async Task TMBCS_21_ValidateUserPasswordAsync_ContainsSystemUserChannels()
    {
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = "testuser",
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.SystemUserChannels, Is.Not.Null);
            Assert.That(response.Resultado.SystemUserChannels, Is.Not.Empty);
            Assert.That(response.Resultado.SystemUserChannels[0].Blocked, Is.False);
            Assert.That(response.Resultado.SystemUserChannels[0].Channel, Is.Not.Null);
            Assert.That(response.Resultado.SystemUserChannels[0].Channel.Mnemonic, Is.EqualTo("BANCAMOBILE"));
        });
    }

    [Test]
    public async Task TMBCS_22_ValidateUserAsync_UsesCustomerInAlias()
    {
        var customerName = "custom.user";
        var request = new ValidateUserRequest("session123", customerName, "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.That(response.Resultado.Authentication.DynamicAuthenticacion.Alias, Is.EqualTo(customerName));
    }

    [Test]
    public async Task TMBCS_23_ValidateUserPasswordAsync_UsesKeyAliasInResponse()
    {
        var keyAlias = "custom.alias";
        var request = new ValidateUserPasswordRequest
        {
            SessionId = "session123",
            KeyAlias = keyAlias,
            KeyValue = "password123"
        };

        var response = await Service.ValidateUserPasswordAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.Customer.Authentications[0].Alias, Is.EqualTo(keyAlias));
            Assert.That(response.Resultado.Customer.Authentications[0].DynamicAuthenticacion.Alias, Is.EqualTo(keyAlias));
        });
    }

    [Test]
    public async Task TMBCS_24_AllMethods_ReturnImmediately()
    {
        var startTime = DateTime.UtcNow;

        await Service.GetAccessTokenAsync();
        await Service.ValidateUserAsync(new ValidateUserRequest("s", "u", "", "en_PA"));
        await Service.ValidateUserPasswordAsync(new ValidateUserPasswordRequest { SessionId = "s", KeyAlias = "a", KeyValue = "v" });
        await Service.TransferBetweenAccountsAsync(new TransferBetweenAccountsRequest { Canal = "MB", Fecha = DateTimeOffset.UtcNow, Tipo = "TRF" });

        var elapsed = DateTime.UtcNow - startTime;

        Assert.That(elapsed.TotalMilliseconds, Is.LessThan(100));
    }

    [Test]
    public async Task TMBCS_25_ValidateUserAsync_ContainsAvatar()
    {
        var request = new ValidateUserRequest("session123", "testuser", "", "en_PA");

        var response = await Service.ValidateUserAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Resultado.Customer.Avatar, Is.Not.Null);
            Assert.That(response.Resultado.Customer.Avatar.AvatarNumber, Is.EqualTo("39"));
        });
    }
}
