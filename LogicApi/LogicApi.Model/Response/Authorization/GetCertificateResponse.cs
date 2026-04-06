namespace LogicApi.Model.Response.Authorization;

/// <summary>
/// Item de certificado
/// </summary>
/// <param name="HashEncrypt"></param>
/// <param name="HashEncryptSign"></param>
public record CertificateValidationItem(string HashEncrypt, string HashEncryptSign);

/// <summary>
/// Respuesta de validación de certificado
/// </summary>
public class GetCertificateResponse
{
    /// <summary>
    /// Certificados
    /// </summary>
    public CertificateValidationItem Certificate { get; set; }

    /// <summary>
    /// Validar Hash
    /// </summary>
    public bool ValidateHash { get; set; }

    /// <summary>
    /// Mensaje al Usuario
    /// </summary>
    public string UserMessage { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetCertificateResponse(CertificateValidationItem certificate, bool validateHash)
    {
        Certificate = certificate;
        ValidateHash = validateHash;
    }
}
