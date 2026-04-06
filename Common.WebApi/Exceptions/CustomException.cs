using Common.WebApi.Messages;

namespace Common.WebApi.Exceptions;

/// <summary>
/// Custom exception
/// </summary>
public class CustomException(MessageCodes code, string message = null) : Exception(message)
{
    /// <summary>
    /// Código de respuesta del error
    /// </summary>
    public MessageCodes MessageCode { get; set; } = code;
}
