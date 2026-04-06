namespace PersistenceDb.Models.Models;
using PersistenceDb.Models.Enums;

/// <summary>
/// Intenta agregar una entidad
/// </summary>
public class GenericDatabaseResponse<T>
{
    /// <summary>
    /// tipo de error
    /// </summary>
    /// <value></value>
    public ErrorDatabaseType? ErrorType { get; set; }

    /// <summary>
    /// Entidad
    /// </summary>
    /// <value></value>
    public T Entity { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="entity"></param>
    public GenericDatabaseResponse(T entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="errorType"></param>
    public GenericDatabaseResponse(ErrorDatabaseType? errorType)
    {
        ErrorType = errorType;
    }
}