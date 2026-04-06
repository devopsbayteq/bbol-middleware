using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PersistenceDb.Authentication;
using PersistenceDb.Core;
using PersistenceDb.Repository.Interfaces.Authentication;
using PersistenceDb.Repository.Interfaces.Core;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace PersistenceDb.UnitOfWork;

public class MainUnitOfWork(
    ILoggerFactory loggerFactory,
    IConfiguration configuration,
    IDbContextFactory<PersistenceContext> dbContextFactory) : UnitOfWork(loggerFactory, configuration, dbContextFactory), IUnitOfWork
{
   
    // Authentication Repositories
    private IDeviceRepository _deviceRepository;
    private IUserRepository _userRepository;
    private IAccountUserRepository _accountUserRepository;
    private IUserDeviceChallengeRepository _userDeviceChallengeRepository;
    private IBeneficiaryRepository _beneficiaryRepository;
    private ITransactionRepository _transactionRepository;
 
   
    public IDeviceRepository DeviceRepository => _deviceRepository ??= 
        new DeviceRepository(Context, LoggerFactory.CreateLogger<DeviceRepository>());

    public IUserRepository UserRepository => _userRepository ??= 
        new UserRepository(Context, LoggerFactory.CreateLogger<UserRepository>());

    public IAccountUserRepository AccountUserRepository => _accountUserRepository ??=
        new AccountUserRepository(Context, LoggerFactory.CreateLogger<AccountUserRepository>());

    public IUserDeviceChallengeRepository UserDeviceChallengeRepository => _userDeviceChallengeRepository ??=
        new UserDeviceChallengeRepository(Context, LoggerFactory.CreateLogger<UserDeviceChallengeRepository>());

    public IBeneficiaryRepository BeneficiaryRepository => _beneficiaryRepository ??=
        new BeneficiaryRepository(Context, LoggerFactory.CreateLogger<BeneficiaryRepository>());

    public ITransactionRepository TransactionRepository => _transactionRepository ??=
        new TransactionRepository(Context, LoggerFactory.CreateLogger<TransactionRepository>());
}
