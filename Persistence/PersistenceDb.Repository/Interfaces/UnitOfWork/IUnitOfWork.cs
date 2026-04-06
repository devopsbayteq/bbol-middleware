
using PersistenceDb.Repository.Interfaces.Authentication;
using PersistenceDb.Repository.Interfaces.Core;
namespace PersistenceDb.Repository.Interfaces.UnitOfWork;
public interface IUnitOfWork : IUnitOfWorkBase
{

    // Authentication Repositories
    IDeviceRepository DeviceRepository { get; }
    IUserRepository UserRepository { get; }

    // Core Repositories
    IAccountUserRepository AccountUserRepository { get; }
    IUserDeviceChallengeRepository UserDeviceChallengeRepository { get; }
    IBeneficiaryRepository BeneficiaryRepository { get; }
    ITransactionRepository TransactionRepository { get; }
}