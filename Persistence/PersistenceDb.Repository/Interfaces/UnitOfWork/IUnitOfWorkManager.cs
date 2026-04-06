namespace PersistenceDb.Repository.Interfaces.UnitOfWork;

public interface IUnitOfWorkManager
{
    IUnitOfWork GetNewUnitOfWork();
}