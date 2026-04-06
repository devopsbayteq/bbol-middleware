namespace ApiCore.Models;
/// <summary>
/// Log
/// </summary>
public class LogRequestModel
{
    /// <summary>
    /// Mensaje a guardar en el log
    /// </summary>
    public string LogMessage { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje a guardar en el log
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje a guardar en el log
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Modelo a guardar en el log
    /// </summary>
    public object ModelToLog { get; set; }
    /// <summary>
    /// Nivel de Log
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

