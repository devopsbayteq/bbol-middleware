using Common.WebApi.Attributes;

namespace BankCore.Integration.Models.User;

public class ValidateUserRequest
{
    public string ServiceId { get; private set; } = "processCustomerLogin";
    public string ServiceVersion { get; private set; } = "1.0";
    public string SessionId { get; set; }

    /// <summary>
    /// Constructor de la solicitud de validación de usuario
    /// </summary>
    /// <param name="sessionId">Id de sesión</param>
    /// <param name="customer">Usuario</param>
    /// <param name="address">Dirección</param>
    /// <param name="locale">Locale</param>
    public ValidateUserRequest(string sessionId, string customer, string address, string locale)
    {
        SessionId = sessionId;
        Customer = customer;
        Address = address;
        Locale = locale;
    }

    public string TargetChannel { get; set; } = "MB";
    public string ExecutingChannel { get; set; } = "MB";
    [IgnoreSensible]
    public string Address { get; set; }
    public string Locale { get; set; }
    public string CmmDispatchDate { get; set; } = "20240123120021380-0500";
    [IgnoreSensible]
    public string Customer { get; set; }
    public string Channel { get; set; } = "MB";
}
