namespace Common.WebApi.ArtificialIntelligence;

public class ArtificialIntelligenceException : Exception
{
    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="message"></param>
    public ArtificialIntelligenceException(string message) : base(message)
    {
    }


    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ArtificialIntelligenceException(string message, Exception innerException) : base(message, innerException)
    {
    }


    /// <summary>
    /// Constructor 
    /// </summary>
    public ArtificialIntelligenceException()
    {

    }
}