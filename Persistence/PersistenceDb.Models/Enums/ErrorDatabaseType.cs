namespace PersistenceDb.Models.Enums;

/// <summary>
/// Tipo de error de base de datos
/// </summary>
public enum ErrorDatabaseType
{
    /// <summary>
    ///  Error con llave primaria duplicada
    /// </summary>
    PrimaryKeyDuplicate,

    /// <summary>
    /// Error no mapeado
    /// </summary>
    ErrorNotMapped,
}