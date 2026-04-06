using PersistenceDb.Models.Core;

namespace PersistenceDb.Repository.Interfaces.Core;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    /// <summary>
    /// Obtiene el monto total de las transacciones por cuenta.
    /// </summary>
    /// <param name="accountGuids"></param>
    /// <returns></returns>
    Task<Dictionary<Guid, decimal>> GetAmountByAccountsAsync(List<Guid> accountGuids);
}
