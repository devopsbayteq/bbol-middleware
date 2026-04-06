using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.Core;

namespace PersistenceDb.Core;

public class TransactionRepository(
    PersistenceContext dbContext,
    ILogger<TransactionRepository> logger
    ) : GenericRepository<Transaction>(dbContext, logger), ITransactionRepository
{

    public async Task<Dictionary<Guid, decimal>> GetAmountByAccountsAsync(List<Guid> accountGuids)
    {
        return (await Context.Transaction.Where(t => accountGuids.Contains(t.AccountGuid))
            .GroupBy(t => t.AccountGuid)
            .Select(g => new { AccountGuid = g.Key, Amount = g.Sum(t => t.Amount) })
            .ToListAsync()
            .ConfigureAwait(false)).ToDictionary(g => g.AccountGuid, g => g.Amount);
    }
}
