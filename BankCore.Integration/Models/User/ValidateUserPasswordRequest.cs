using Common.WebApi.Attributes;

namespace BankCore.Integration.Models.User;

public class ValidateUserPasswordRequest
{
    public string TargetChannel { get; set; } = "MB";
    public string ExecutingChannel { get; set; } = "MB";
    public string Address { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string Issuer { get; set; } = "CyberbankMultichannelManager";
    [IgnoreSensible]
    public string KeyAlias { get; set; } = string.Empty;
    [IgnoreSensible]
    public string KeyValue { get; set; } = string.Empty;
    public string AuthenticationMethodSchema { get; set; } = "uriTech=ESQUEMA_USER_PASSWORD@21348";
    [IgnoreSensible]
    public string CustomerTokenValue { get; set; } = "ENC(hGMBZKMM6XzpWY4yGWdodoAB93O0QIBwwMCzaGqlAw46egYDmJQIsrL/lVQ0rOPdqiYbe4tVZn/HBMcFajXYBNHtmhuOihER)";
    public string ServiceId { get; set; } = "processSystemUserLogin";
    public string SessionId { get; set; } = string.Empty;
}
