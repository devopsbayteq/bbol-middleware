using System.Text.Json.Serialization;
using BankCore.Integration.Models.Common;

namespace BankCore.Integration.Models.User;

/// <summary>
/// Respuesta de <c>valida-clave-usuarios</c> (processSystemUserLogin).
/// </summary>
public sealed class ValidateUserPasswordResponse
{
    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("resultado")]
    public ValidateUserPasswordResultado Resultado { get; set; }
}

public sealed class ValidateUserPasswordResultado
{
    [JsonPropertyName("header")]
    public BankCoreResponseHeader Header { get; set; }

    [JsonPropertyName("generic")]
    public ValidateUserPasswordGeneric Generic { get; set; }

    [JsonPropertyName("customer")]
    public ValidateUserPasswordCustomer Customer { get; set; }

    [JsonPropertyName("administrator")]
    public string Administrator { get; set; } = string.Empty;

    [JsonPropertyName("authentications")]
    public List<ValidateUserPasswordAuthenticationItem> Authentications { get; set; }

    [JsonPropertyName("canLogin")]
    public string CanLogin { get; set; } = string.Empty;

    [JsonPropertyName("createDate")]
    public string CreateDate { get; set; } = string.Empty;

    [JsonPropertyName("electronicsContact")]
    public BankCoreElectronicsContact ElectronicsContact { get; set; }

    [JsonPropertyName("firstLogin")]
    public string FirstLogin { get; set; } = string.Empty;

    [JsonPropertyName("institution")]
    public ValidateUserPasswordInstitution Institution { get; set; }

    [JsonPropertyName("metaStatus")]
    public BankCoreMnemonicStatus MetaStatus { get; set; }

    [JsonPropertyName("roleTypes")]
    public List<ValidateUserPasswordRoleType> RoleTypes { get; set; }

    [JsonPropertyName("status")]
    public BankCoreMnemonicStatus Status { get; set; }

    [JsonPropertyName("systemUserChannels")]
    public List<ValidateUserPasswordSystemUserChannel> SystemUserChannels { get; set; }

    [JsonPropertyName("systemUserType")]
    public ValidateUserPasswordSystemUserType SystemUserType { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordGeneric
{
    [JsonPropertyName("canLogin")]
    public string CanLogin { get; set; } = string.Empty;

    [JsonPropertyName("identificationNumberMasked")]
    public string IdentificationNumberMasked { get; set; } = string.Empty;

    [JsonPropertyName("hasAvatar")]
    public string HasAvatar { get; set; } = string.Empty;

    [JsonPropertyName("hasSecretQuestion")]
    public string HasSecretQuestion { get; set; } = string.Empty;

    [JsonPropertyName("keyChange")]
    public string KeyChange { get; set; } = string.Empty;

    [JsonPropertyName("lastAccessDate")]
    public string LastAccessDate { get; set; } = string.Empty;

    [JsonPropertyName("keyChangeDate")]
    public string KeyChangeDate { get; set; } = string.Empty;

    [JsonPropertyName("usernameChange")]
    public string UsernameChange { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordCustomer
{
    [JsonPropertyName("authentications")]
    public List<ValidateUserPasswordCustomerAuthentication> Authentications { get; set; }

    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("firstName1")]
    public string FirstName1 { get; set; } = string.Empty;

    [JsonPropertyName("bankCustomerCode")]
    public string BankCustomerCode { get; set; } = string.Empty;

    [JsonPropertyName("electronicsContact")]
    public BankCoreElectronicsContact ElectronicsContact { get; set; }

    [JsonPropertyName("identificationNumber")]
    public string IdentificationNumber { get; set; } = string.Empty;

    [JsonPropertyName("identificationType")]
    public ValidateUserPasswordIdentificationType IdentificationType { get; set; }

    [JsonPropertyName("isEmployee")]
    public string IsEmployee { get; set; } = string.Empty;

    [JsonPropertyName("lastName1")]
    public string LastName1 { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public BankCoreTokenValue Token { get; set; }
}

public sealed class ValidateUserPasswordIdentificationType
{
    [JsonPropertyName("mnemonic")]
    public string Mnemonic { get; set; } = string.Empty;

    [JsonPropertyName("longDesc")]
    public string LongDesc { get; set; } = string.Empty;

    [JsonPropertyName("shortDesc")]
    public string ShortDesc { get; set; } = string.Empty;

    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;

    [JsonPropertyName("internalValues")]
    public string InternalValues { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordCustomerAuthentication
{
    [JsonPropertyName("activationRequire")]
    public string ActivationRequire { get; set; } = string.Empty;

    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("authenticationId")]
    public string AuthenticationId { get; set; } = string.Empty;

    [JsonPropertyName("dynamicAuthenticacion")]
    public ValidateUserPasswordCustomerDynamicAuth DynamicAuthenticacion { get; set; }
}

public sealed class ValidateUserPasswordCustomerDynamicAuth
{
    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("authenticationType")]
    public ValidateUserPasswordCustomerAuthType AuthenticationType { get; set; }

    [JsonPropertyName("login")]
    public bool Login { get; set; }
}

public sealed class ValidateUserPasswordCustomerAuthType
{
    [JsonPropertyName("authenticatesProfiling")]
    public string AuthenticatesProfiling { get; set; } = string.Empty;

    [JsonPropertyName("authenticationTypeId")]
    public string AuthenticationTypeId { get; set; } = string.Empty;

    [JsonPropertyName("challengeRequired")]
    public string ChallengeRequired { get; set; } = string.Empty;

    [JsonPropertyName("createDate")]
    public string CreateDate { get; set; } = string.Empty;

    [JsonPropertyName("metaStatus")]
    public BankCoreMnemonicStatus MetaStatus { get; set; }

    [JsonPropertyName("status")]
    public BankCoreMnemonicStatus Status { get; set; }

    [JsonPropertyName("weighting")]
    public string Weighting { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordAuthenticationItem
{
    [JsonPropertyName("authenticationSchema")]
    public ValidateUserPasswordAuthSchema AuthenticationSchema { get; set; }

    [JsonPropertyName("channel")]
    public ValidateUserPasswordChannel Channel { get; set; }

    [JsonPropertyName("dynamicAuthenticacion")]
    public ValidateUserPasswordDynamicAuthItem DynamicAuthenticacion { get; set; }
}

public sealed class ValidateUserPasswordAuthSchema
{
    [JsonPropertyName("authenticationSchemaId")]
    public string AuthenticationSchemaId { get; set; } = string.Empty;

    [JsonPropertyName("metaStatus")]
    public BankCoreMnemonicStatus MetaStatus { get; set; }

    [JsonPropertyName("status")]
    public ValidateUserPasswordSchemaStatus Status { get; set; }
}

public sealed class ValidateUserPasswordSchemaStatus
{
    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordChannel
{
    [JsonPropertyName("identificationChannel")]
    public BankCoreIdentificationChannel IdentificationChannel { get; set; }

    [JsonPropertyName("mnemonic")]
    public string Mnemonic { get; set; } = string.Empty;

    [JsonPropertyName("shortDesc")]
    public string ShortDesc { get; set; } = string.Empty;

    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;

    [JsonPropertyName("internalValues")]
    public string InternalValues { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordDynamicAuthItem
{
    [JsonPropertyName("authenticationType")]
    public ValidateUserPasswordDynamicAuthType AuthenticationType { get; set; }
}

public sealed class ValidateUserPasswordDynamicAuthType
{
    [JsonPropertyName("authenticatesProfiling")]
    public string AuthenticatesProfiling { get; set; } = string.Empty;

    [JsonPropertyName("authenticationTypeId")]
    public string AuthenticationTypeId { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("weighting")]
    public string Weighting { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordInstitution
{
    [JsonPropertyName("identificationType")]
    public ValidateUserPasswordIdentificationType IdentificationType { get; set; }

    [JsonPropertyName("identificationNumber")]
    public string IdentificationNumber { get; set; } = string.Empty;

    [JsonPropertyName("authenticationRequired")]
    public string AuthenticationRequired { get; set; } = string.Empty;

    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("sIntitutionId")]
    public string SIntitutionId { get; set; } = string.Empty;

    [JsonPropertyName("segment")]
    public ValidateUserPasswordSegment Segment { get; set; }
}

public sealed class ValidateUserPasswordSegment
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sSegmentId")]
    public string SSegmentId { get; set; } = string.Empty;

    [JsonPropertyName("bankingType")]
    public ValidateUserPasswordBankingType BankingType { get; set; }
}

public sealed class ValidateUserPasswordBankingType
{
    [JsonPropertyName("mnemonic")]
    public string Mnemonic { get; set; } = string.Empty;

    [JsonPropertyName("longDesc")]
    public string LongDesc { get; set; } = string.Empty;

    [JsonPropertyName("shortDesc")]
    public string ShortDesc { get; set; } = string.Empty;

    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;

    [JsonPropertyName("internalValues")]
    public string InternalValues { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordRoleType
{
    [JsonPropertyName("roleTypeId")]
    public string RoleTypeId { get; set; } = string.Empty;

    [JsonPropertyName("shortDesc")]
    public string ShortDesc { get; set; } = string.Empty;

    [JsonPropertyName("systemUserType")]
    public ValidateUserPasswordSystemUserType SystemUserType { get; set; }
}

public sealed class ValidateUserPasswordSystemUserType
{
    [JsonPropertyName("mnemonic")]
    public string Mnemonic { get; set; } = string.Empty;

    [JsonPropertyName("shortDesc")]
    public string ShortDesc { get; set; } = string.Empty;

    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;

    [JsonPropertyName("internalValues")]
    public string InternalValues { get; set; } = string.Empty;
}

public sealed class ValidateUserPasswordSystemUserChannel
{
    [JsonPropertyName("blocked")]
    public bool Blocked { get; set; }

    [JsonPropertyName("channel")]
    public ValidateUserPasswordChannel Channel { get; set; }

    [JsonPropertyName("lastAccessDate")]
    public string LastAccessDate { get; set; } = string.Empty;
}
