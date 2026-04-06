namespace PersistenceDb.CustomException;

/// <summary>
/// Blob Exception
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="message"></param>

public class CustomPersistenceException(string message, Exception innerException = null) : Exception(message, innerException)
{
}
