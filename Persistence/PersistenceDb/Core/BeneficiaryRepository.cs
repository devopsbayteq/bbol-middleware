using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Core;
using PersistenceDb.Repository.Interfaces.Core;

namespace PersistenceDb.Core;

public class BeneficiaryRepository(
    PersistenceContext dbContext,
    ILogger<BeneficiaryRepository> logger
    ) : GenericRepository<Beneficiary>(dbContext, logger), IBeneficiaryRepository
{
}
