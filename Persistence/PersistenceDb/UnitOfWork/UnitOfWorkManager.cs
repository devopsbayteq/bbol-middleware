using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace PersistenceDb.UnitOfWork;

/// <summary>
/// Constructor
/// </summary>
/// <param name="loggerFactory"></param>
public class UnitOfWorkManager(ILoggerFactory loggerFactory, IConfiguration configuration, IDbContextFactory<PersistenceContext> dbContextFactory) : IUnitOfWorkManager
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly IDbContextFactory<PersistenceContext> _dbContextFactory = dbContextFactory;

    public IUnitOfWork GetNewUnitOfWork()
        => new MainUnitOfWork(_loggerFactory, _configuration, _dbContextFactory);
}
