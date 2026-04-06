using System.Text.Json;
using System.Text.Json.Serialization;
using BankCore.Integration.Models.Common;

namespace BankCore.Integration.Models.User;

/// <summary>
/// Respuesta de <c>valida-usuarios</c> (processCustomerLogin).
/// </summary>
public sealed class ValidateUserResponse
{
    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("resultado")]
    public ValidateUserResultado Resultado { get; set; }
}

public sealed class ValidateUserResultado
{
    [JsonPropertyName("header")]
    public BankCoreResponseHeader Header { get; set; }

    [JsonPropertyName("authentication")]
    public ValidateUserAuthentication Authentication { get; set; }

    [JsonPropertyName("customer")]
    public ValidateUserCustomer Customer { get; set; }

    [JsonPropertyName("electronicsContact")]
    public BankCoreElectronicsContact ElectronicsContact { get; set; }
}

public sealed class ValidateUserAuthentication
{
    [JsonPropertyName("activationRequired")]
    public string ActivationRequired { get; set; } = string.Empty;

    [JsonPropertyName("authenticationSchema")]
    public ValidateUserAuthenticationSchema AuthenticationSchema { get; set; }

    [JsonPropertyName("dynamicAuthenticacion")]
    public ValidateUserDynamicAuthentication DynamicAuthenticacion { get; set; }
}

public sealed class ValidateUserAuthenticationSchema
{
    [JsonPropertyName("authenticationSchemaId")]
    public string AuthenticationSchemaId { get; set; } = string.Empty;
}

public sealed class ValidateUserDynamicAuthentication
{
    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("authenticationType")]
    public ValidateUserAuthenticationType AuthenticationType { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;
}

public sealed class ValidateUserAuthenticationType
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

public sealed class ValidateUserCustomer
{
    [JsonPropertyName("avatar")]
    public ValidateUserAvatar Avatar { get; set; }

    [JsonPropertyName("token")]
    public BankCoreTokenValue Token { get; set; }

    [JsonPropertyName("identificationNumber")]
    public string IdentificationNumber { get; set; } = string.Empty;

    [JsonPropertyName("firstName1")]
    public string FirstName1 { get; set; } = string.Empty;

    [JsonPropertyName("lastName1")]
    public string LastName1 { get; set; } = string.Empty;

    [JsonPropertyName("thirdCode")]
    public string ThirdCode { get; set; } = string.Empty;
}

public sealed class ValidateUserAvatar
{
    [JsonPropertyName("avatarNumber")]
    public string AvatarNumber { get; set; } = string.Empty;
}
