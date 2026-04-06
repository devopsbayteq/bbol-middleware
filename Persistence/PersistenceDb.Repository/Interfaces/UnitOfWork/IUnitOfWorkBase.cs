using System.Data;
namespace PersistenceDb.Repository.Interfaces.UnitOfWork;

public interface IUnitOfWorkBase : IDisposable
{
    /// <summary>
    /// Inicia una transacción
    /// </summary>
    /// <returns></returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit de una transacción
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();

    /// <summary>
    /// Roll Back
    /// </summary>
    /// <returns></returns>
    Task RollBackAsync();

    /// <summary>
    /// Métodos para generar transacciones
    /// </summary>
    /// <param name="process"></param>
    /// <param name="Commit"></param>
    /// <returns></returns>
    Task BeginTransactionStrategyAsync(
        Func<Task> process,
        bool autoCommit = true);

    /// <summary>
    /// Genera una función para ejecutar como transacción
    /// </summary>
    /// <param name="process"></param>
    /// <param name="Commit"></param>
    /// <param name="isolationLevel"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> BeginTransactionStrategyAsync<TResult>(
        Func<Task<TResult>> process,
        bool autoCommit = true,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}
