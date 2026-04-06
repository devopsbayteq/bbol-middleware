using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Authentication;
using PersistenceDb.Repository.Interfaces.Authentication;

namespace PersistenceDb.Authentication;

public class UserRepository(PersistenceContext dbContext, ILogger<UserRepository> logger) 
: GenericRepository<User>(dbContext, logger), IUserRepository
{
}