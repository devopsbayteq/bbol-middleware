namespace PersistenceDb.Repository.Models;
/// <summary>
/// Interfaz para paginación 
/// </summary>
public interface IPaginator<TModel>
{
    /// <summary>
    /// Items del paginado
    /// </summary>
    /// <value></value>
    IEnumerable<TModel> Items { get; set; }

    /// <summary>
    /// Número total de Items
    /// </summary>
    /// <value></value>
    int TotalItems { get; set; }
}