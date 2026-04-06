using PersistenceDb.Repository.Models;

namespace PersistenceDb.Model;

/// <summary>
/// Implementación de Paginator Model 
/// </summary>
public class PaginatorModel<TModel> : IPaginator<TModel>
{
    /// <summary>
    /// Número total de Items
    /// </summary>
    /// <value></value>
    public int TotalItems { get; set; }

    /// <summary>
    /// Items del paginado
    /// </summary>
    /// <value></value>
    public IEnumerable<TModel> Items { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="items"></param>
    /// <param name="totalItems"></param>
    public PaginatorModel(IEnumerable<TModel> items, int totalItems = 0)
    {
        Items = items;
        TotalItems = totalItems;
    }
}