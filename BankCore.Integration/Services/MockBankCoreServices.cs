using System.Text.Json;
using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Common;
using BankCore.Integration.Models.Transfer;
using BankCore.Integration.Models.User;

namespace BankCore.Integration.Services;

public class MockBankCoreServices : IBankCoreServices
{
    private const string FalseValue = "false";
    private const string AliasValue = "isAlias=false;";

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        => Task.FromResult("mock-access-token");

    public Task<ValidateUserResponse> ValidateUserAsync(ValidateUserRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(CreateValidateUserResponse(request));

    public Task<ValidateUserPasswordResponse> ValidateUserPasswordAsync(ValidateUserPasswordRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(CreateValidateUserPasswordResponse(request));

    public Task<TransferBetweenAccountsResponse> TransferBetweenAccountsAsync(TransferBetweenAccountsRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(CreateTransferBetweenAccountsResponse(request));

    private static ValidateUserResponse CreateValidateUserResponse(ValidateUserRequest request)
    {
        var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
            ? "MOCK-SESSION"
            : request.SessionId;

        return new ValidateUserResponse
        {
            Codigo = "0",
            Mensaje = "Transaccion exitosa (mock)",
            Resultado = new ValidateUserResultado
            {
                Header = CreateMockHeader(sessionId, "processCustomerLogin"),
                Authentication = new ValidateUserAuthentication
                {
                    ActivationRequired = FalseValue,
                    AuthenticationSchema = new ValidateUserAuthenticationSchema
                    {
                        AuthenticationSchemaId = "uriTech=ESQUEMA_USER_PASSWORD@mock"
                    },
                    DynamicAuthenticacion = new ValidateUserDynamicAuthentication
                    {
                        Alias = request.Customer ?? "mock-user",
                        AuthenticationType = new ValidateUserAuthenticationType
                        {
                            AuthenticatesProfiling = "true",
                            AuthenticationTypeId = "uriTech=METODO_AUTENTICACION@3;uriSFB=USUARIO_CLAVE;",
                            ChallengeRequired = "USUARIO_CLAVE",
                            CreateDate = "2017-01-20T13:11:47-05:00",
                            MetaStatus = ActiveMnemonicStatus(),
                            Status = ActiveMnemonicStatus(),
                            Weighting = "0"
                        },
                        Login = bool.TrueString.ToLowerInvariant()
                    }
                },
                Customer = new ValidateUserCustomer
                {
                    Avatar = new ValidateUserAvatar { AvatarNumber = "39" },
                    Token = new BankCoreTokenValue { Value = "MOCK-SYNTHETIC-CUSTOMER-TOKEN-NOT-SECRET" },
                    IdentificationNumber = "0000000000",
                    FirstName1 = "MOCK",
                    LastName1 = "USUARIO",
                    ThirdCode = "MOCK001"
                },
                ElectronicsContact = new BankCoreElectronicsContact
                {
                    EmailAddressComplete = "mock@bolivariano.com"
                }
            }
        };
    }

    private static ValidateUserPasswordResponse CreateValidateUserPasswordResponse(ValidateUserPasswordRequest request)
    {
        var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
            ? "MOCK-SESSION"
            : request.SessionId;
        var alias = string.IsNullOrWhiteSpace(request.KeyAlias) ? "mock-alias" : request.KeyAlias;
        var now = DateTimeOffset.Now;

        var identificationType = new ValidateUserPasswordIdentificationType
        {
            Mnemonic = "C",
            LongDesc = "Cédula",
            ShortDesc = "Cédula",
            OriginalCodes = "originalCode=CEDULA_DE_IDENTIDAD;",
            InternalValues = AliasValue
        };

        var customerAuthType = new ValidateUserPasswordCustomerAuthType
        {
            AuthenticatesProfiling = "true",
            AuthenticationTypeId = "uriTech=METODO_AUTENTICACION@3;uriSFB=USUARIO_CLAVE;",
            ChallengeRequired = "",
            CreateDate = "",
            MetaStatus = ActiveMnemonicStatus(),
            Status = ActiveMnemonicStatus(),
            Weighting = "0"
        };

        var customerDynamic = new ValidateUserPasswordCustomerDynamicAuth
        {
            Alias = alias,
            AuthenticationType = customerAuthType,
            Login = false
        };

        var customerAuth = new ValidateUserPasswordCustomerAuthentication
        {
            ActivationRequire = FalseValue,
            Alias = alias,
            AuthenticationId = "uriTech=ESQUEMA_USER_PASSWORD@mock",
            DynamicAuthenticacion = customerDynamic
        };

        var otpAuthItem = new ValidateUserPasswordAuthenticationItem
        {
            AuthenticationSchema = new ValidateUserPasswordAuthSchema
            {
                AuthenticationSchemaId = "uriTech=ESQUEMA_TECH_TOKEN@mock-otp",
                MetaStatus = ActiveMnemonicStatus(),
                Status = new ValidateUserPasswordSchemaStatus { OriginalCodes = "A" }
            },
            Channel = new ValidateUserPasswordChannel
            {
                IdentificationChannel = new BankCoreIdentificationChannel
                {
                    Mnemonic = "uriTech=CANAL@111",
                    LongDesc = "",
                    ShortDesc = "",
                    OriginalCodes = "originalCode=uriTech=CANAL@111;",
                    InternalValues = AliasValue
                },
                Mnemonic = "BANCAMOBILE",
                ShortDesc = "Banca mobile",
                OriginalCodes = "originalCode=BANCAMOBILE;",
                InternalValues = AliasValue
            },
            DynamicAuthenticacion = new ValidateUserPasswordDynamicAuthItem
            {
                AuthenticationType = new ValidateUserPasswordDynamicAuthType
                {
                    AuthenticatesProfiling = FalseValue,
                    AuthenticationTypeId = "uriTech=METODO_AUTENTICACION@4;uriSFB=OTP;",
                    Key = "OTP",
                    Weighting = "3"
                }
            }
        };

        var systemUserType = new ValidateUserPasswordSystemUserType
        {
            Mnemonic = "INSTITUTION_OPERATOR",
            ShortDesc = "Operador de Empresa",
            OriginalCodes = "originalCode=OE;",
            InternalValues = AliasValue
        };

        return new ValidateUserPasswordResponse
        {
            Codigo = "0",
            Mensaje = "Transaccion exitosa (mock)",
            Resultado = new ValidateUserPasswordResultado
            {
                Header = CreateMockHeader(sessionId, "processSystemUserLogin"),
                Generic = new ValidateUserPasswordGeneric
                {
                    CanLogin = "true",
                    IdentificationNumberMasked = "00******00",
                    HasAvatar = "true",
                    HasSecretQuestion = FalseValue,
                    KeyChange = FalseValue,
                    LastAccessDate = now.ToString("yyyyMMddHHmmssfff") + "-0500",
                    KeyChangeDate = now.AddYears(1).ToString("yyyyMMddHHmmssfff") + "-0500",
                    UsernameChange = FalseValue,
                    Result = "true"
                },
                Customer = new ValidateUserPasswordCustomer
                {
                    Authentications = [customerAuth],
                    CustomerId = "uriTech=PERSONA@mock",
                    FirstName1 = "MOCK",
                    BankCustomerCode = "MOCK001",
                    ElectronicsContact = new BankCoreElectronicsContact
                    {
                        EmailAddressComplete = "mock@bolivariano.com"
                    },
                    IdentificationNumber = "0000000000",
                    IdentificationType = identificationType,
                    IsEmployee = "true",
                    LastName1 = "USUARIO",
                    Token = new BankCoreTokenValue { Value = "MOCK-SYNTHETIC-PASSWORD-FLOW-TOKEN-NOT-SECRET" }
                },
                Administrator = "true",
                Authentications = [otpAuthItem],
                CanLogin = "true",
                CreateDate = "20171110000000000-0500",
                ElectronicsContact = new BankCoreElectronicsContact
                {
                    EmailAddressComplete = "mock@bolivariano.com"
                },
                FirstLogin = "true",
                Institution = new ValidateUserPasswordInstitution
                {
                    IdentificationType = identificationType,
                    IdentificationNumber = "0000000000",
                    AuthenticationRequired = FalseValue,
                    CustomerName = "MOCK",
                    SIntitutionId = "uriTech=EMPRESA@mock",
                    Segment = new ValidateUserPasswordSegment
                    {
                        Name = "SILVER-SILVER",
                        SSegmentId = "uriTech=SEGMENTO@mock",
                        BankingType = new ValidateUserPasswordBankingType
                        {
                            Mnemonic = "RETAIL_BANKING",
                            LongDesc = "Banca Individuos",
                            ShortDesc = "Banca Individuos",
                            OriginalCodes = "originalCode=BI;",
                            InternalValues = AliasValue
                        }
                    }
                },
                MetaStatus = ActiveMnemonicStatus(),
                RoleTypes =
                [
                    new ValidateUserPasswordRoleType
                    {
                        RoleTypeId = "uriTech=ROL_OPERADOR@mock",
                        ShortDesc = "OPERADOR_COMUN",
                        SystemUserType = systemUserType
                    }
                ],
                Status = ActiveMnemonicStatus(),
                SystemUserChannels =
                [
                    new ValidateUserPasswordSystemUserChannel
                    {
                        Blocked = false,
                        Channel = new ValidateUserPasswordChannel
                        {
                            IdentificationChannel = new BankCoreIdentificationChannel
                            {
                                Mnemonic = "MB",
                                LongDesc = "MOBILE",
                                ShortDesc = "MOBILE",
                                OriginalCodes = "originalCode=CANAL@111;",
                                InternalValues = AliasValue
                            },
                            Mnemonic = "BANCAMOBILE",
                            ShortDesc = "Banca mobile",
                            OriginalCodes = "originalCode=BANCAMOBILE;",
                            InternalValues = AliasValue
                        },
                        LastAccessDate = now.ToString("yyyyMMddHHmmssfff") + "-0500"
                    }
                ],
                SystemUserType = systemUserType,
                UserId = $"uriTech=OPERADOR_EMPRESA@mock;userName={alias};OWNER=true;"
            }
        };
    }

    private static TransferBetweenAccountsResponse CreateTransferBetweenAccountsResponse(TransferBetweenAccountsRequest request)
    {
        var ts = request.Fecha != default ? request.Fecha : DateTimeOffset.Now;
        var seq = string.IsNullOrWhiteSpace(request.Auditoria?.Secuencial)
            ? $"MOCK{ts:yyyyMMddHHmmssfff}"
            : request.Auditoria.Secuencial;

        return new TransferBetweenAccountsResponse
        {
            FechaProceso = ts.ToString("yyyy-MM-dd"),
            FechaTransaccion = ts.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
            SecuenciaTransaccion = seq
        };
    }

    private static BankCoreResponseHeader CreateMockHeader(string sessionId, string serviceId)
        => new()
        {
            Address = "ip=127.0.0.1;",
            Internals = new BankCoreHeaderInternals
            {
                ServiceProviderEntityName = "MOCK",
                ServiceProviderName = "MOCK"
            },
            ExecutingChannel = new BankCoreExecutingChannel
            {
                Mnemonic = "MB",
                OriginalCodes = "originalCode=MB;",
                InternalValues = AliasValue
            },
            Locale = "en_PA",
            TraceNumber = $"mock.{DateTime.UtcNow.Ticks}",
            ServiceId = serviceId,
            ServiceVersion = "0.00.0",
            SessionId = sessionId,
            ChannelId = "MB"
        };

    private static BankCoreMnemonicStatus ActiveMnemonicStatus()
        => new()
        {
            Mnemonic = "ACTIVE",
            OriginalCodes = "originalCode=ACTIVE;",
            InternalValues = AliasValue
        };
}
