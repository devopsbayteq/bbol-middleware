namespace PersistenceDb.Models.Models;
/// <summary>
/// Control de Versiones de Fileas
/// </summary>
public interface IEntityControlRow
{
    /// <summary>
    /// Control de Filas
    /// </summary>
    /// <value></value>
    DateTime RowControl { get; set; }
}