namespace PersistenceDb.Models.Configuration;
/// <summary>
/// Configuración de conexión a base de datos
/// </summary>
public class DatabaseConfiguration
{
 
    /// <summary>
    /// Conexión
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Time Out
    /// </summary>
    public int CommandTimeOut { get; set; }

    /// Cadena de conexión
    /// </summary>
    /// <value></value>
    public bool EnableSensitiveDataLogging { get; set; }

    /// <summary>
    /// Time out 
    /// </summary>
    /// <value></value>    
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// Secreto Aes
    /// </summary>
    /// <value></value>
    public string AesSecret { get; set; }
}