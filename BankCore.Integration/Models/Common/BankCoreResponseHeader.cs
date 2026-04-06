using System.Text.Json.Serialization;

namespace BankCore.Integration.Models.Common;

/// <summary>
/// Cabecera típica en <c>resultado.header</c> de respuestas BankCore (validación usuario / clave).
/// </summary>
public sealed class BankCoreResponseHeader
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("internals")]
    public BankCoreHeaderInternals Internals { get; set; }

    [JsonPropertyName("executingChannel")]
    public BankCoreExecutingChannel ExecutingChannel { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = string.Empty;

    [JsonPropertyName("traceNumber")]
    public string TraceNumber { get; set; } = string.Empty;

    [JsonPropertyName("serviceId")]
    public string ServiceId { get; set; } = string.Empty;

    [JsonPropertyName("serviceVersion")]
    public string ServiceVersion { get; set; } = string.Empty;

    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [JsonPropertyName("channelId")]
    public string ChannelId { get; set; } = string.Empty;
}

public sealed class BankCoreHeaderInternals
{
    [JsonPropertyName("serviceProviderEntityName")]
    public string ServiceProviderEntityName { get; set; } = string.Empty;

    [JsonPropertyName("serviceProviderName")]
    public string ServiceProviderName { get; set; } = string.Empty;
}

public sealed class BankCoreExecutingChannel
{
    [JsonPropertyName("mnemonic")]
    public string Mnemonic { get; set; } = string.Empty;

    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;

    [JsonPropertyName("internalValues")]
    public string InternalValues { get; set; } = string.Empty;
}

public sealed class BankCoreMnemonicStatus
{
    [JsonPropertyName("mnemonic")]
    public string Mnemonic { get; set; } = string.Empty;

    [JsonPropertyName("originalCodes")]
    public string OriginalCodes { get; set; } = string.Empty;

    [JsonPropertyName("internalValues")]
    public string InternalValues { get; set; } = string.Empty;
}

public sealed class BankCoreElectronicsContact
{
    [JsonPropertyName("emailAddressComplete")]
    public string EmailAddressComplete { get; set; } = string.Empty;
}

public sealed class BankCoreTokenValue
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Canal de identificación en estructuras <c>channel</c> (p. ej. BANCAMOBILE).
/// </summary>
public sealed class BankCoreIdentificationChannel
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
