namespace LogicApi.Model.Response.Security;

/// <summary>
/// Respuesta para challenge biometrico
/// </summary>
public class GenerateBiometricChallengeResponse
{
    /// <summary>
    /// Challenge aleatorio seguro
    /// </summary>
    public string Challenge { get; set; } = string.Empty;
}
