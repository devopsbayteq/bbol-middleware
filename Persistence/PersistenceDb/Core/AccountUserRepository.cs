using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.Core;

namespace PersistenceDb.Core;

/// <summary>
/// Repositorio de cuenta de usuario.
/// </summary>
public class AccountUserRepository(
    PersistenceContext dbContext,
    ILogger<AccountUserRepository> logger
    ) : GenericRepository<AccountUser>(dbContext, logger), IAccountUserRepository
{
}
