using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.Core;

namespace PersistenceDb.Core;

public class UserDeviceChallengeRepository(
    PersistenceContext dbContext,
    ILogger<UserDeviceChallengeRepository> logger)
    : GenericRepository<UserDeviceChallenge>(dbContext, logger), IUserDeviceChallengeRepository
{
}
