namespace BankCore.Integration.Exceptions;

public class BankCoreUserMessageException : Exception
{
    public BankCoreUserMessageException(string message) : base(message)
    {
    }

    public BankCoreUserMessageException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public BankCoreUserMessageException()
    {
    }
}