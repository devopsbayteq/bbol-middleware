using System;
using System.Linq.Expressions;
using PersistenceDb.Models.Enums;
using PersistenceDb.Models.Models;
using PersistenceDb.Repository.Models;

namespace PersistenceDb.Repository.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Obtiene los registros con filtro, orden y top
    /// </summary>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<List<TEntity>> GetByAsync(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType? orderByType = default,
        int? top = null,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Obtiene el primer registro mediante un filtro y con entidades Incluidas (JOIN)
    /// </summary>
    /// <param name="where"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity> GetByFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> where = null,
            params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Obtiene el primer registro mediante un filtro y con entidades Incluidas (JOIN)
    /// </summary>
    /// <param name="where"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TResult> GetFirstOrDefaultGenericAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Obtiene el primer registro mediante una lista de filtros y con entidades Incluidas (JOIN)
    /// </summary>
    /// <param name="where"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TResult> GetFirstOrDefaultGenericAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        List<Expression<Func<TEntity, bool>>> whereClauses);

    /// <summary>
    /// Verifica si existe algún registro en una tabla
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    Task<bool> ExistAnyAsync(Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Obtiene la cantidad de Registros de una Tabla con un filtrado
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Agrega una tentidad
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> AddAsync(TEntity entity, bool autoDetectChangesEnabled = false);

    /// <summary>
    /// Agrega Varias Entidades
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="includeChildren"></param>
    /// <returns></returns>
    Task<int> AddRangeAsync(IEnumerable<TEntity> entity, bool includeChildren = false);

    /// <summary>
    /// Agrega Varias Entidades y optioene también el Id generado
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<List<TEntity>> AddRangeIdentityAsync(List<TEntity> entity);

    /// <summary>
    /// Actualiza Entidades
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Actualiza varias entidades
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> UpdateRangeAsync(IList<TEntity> entities);

    /// <summary>
    /// Elimina una Entidad
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(TEntity entity);

    /// <summary>
    /// Elimina una Entidad
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Obtiene el valor máximo de una tabla
    /// </summary>
    /// <param name="selector"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> GetMaxValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

    /// <summary>
    /// Obtiene el valor Mínimo de una tabla
    /// </summary>
    /// <param name="selector"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> GetMinValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector);

    /// <summary>
    /// Pagina los resultados
    /// </summary>
    /// <param name="itemsByPage"></param>
    /// <param name="page"></param>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderByType"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<IPaginator<TEntity>> GetPaginatorByAsync(
        int itemsByPage,
        int page,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType orderByType = default,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Obtiene un objeto genérico
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderByType"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<List<TResult>> GetGenericAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType orderByType = default);



    /// <summary>
    /// Obtiene los Registros Paginados
    /// </summary>
    /// <param name="itemsByPage"></param>
    /// <param name="page"></param>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderByType"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<IPaginator<TResult>> GetPaginatorGenericAsync<TResult>(
    int itemsByPage,
    int page,
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>> where = null,
    Expression<Func<TEntity, dynamic>> orderBy = null,
    OrderByType orderByType = default);

    /// <summary>
    /// Obtiene los Registros Paginados
    /// </summary>
    /// <param name="itemsByPage"></param>
    /// <param name="page"></param>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderByType"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<IPaginator<TResult>> GetPaginatorGenericAsync<TResult>(
    int itemsByPage,
    int page,
    Expression<Func<TEntity, TResult>> selector,
    List<Expression<Func<TEntity, bool>>> where,
    Expression<Func<TEntity, dynamic>> orderBy = null,
    OrderByType orderByType = default);

    /// <summary>
    /// Obtiene los valores distintos de una columna
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<List<TResult>> DistinctAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Actualiza un registro
    /// </summary>
    /// <param name="propertyExpression"></param>
    /// <param name="value"></param>
    /// <param name="where"></param>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    public Task<int> UpdateByAsync<TProperty>(
        (Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) properties,
        Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Actualiza un registro
    /// </summary>
    /// <param name="propertyExpression"></param>
    /// <param name="value"></param>
    /// <param name="where"></param>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    public Task<int> UpdateByAsync<TProperty, TProperty2>(
        (Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) property,
        (Expression<Func<TEntity, TProperty2>> propertyExpression2, TProperty2 value2) property2,
        Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Actualiza un registro
    /// </summary>
    /// <param name="propertyExpression"></param>
    /// <param name="value"></param>
    /// <param name="where"></param>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    public Task<int> UpdateByAsync<TProperty, TProperty2, TProperty3>(
        (Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) property,
        (Expression<Func<TEntity, TProperty2>> propertyExpression2, TProperty2 value2) property2,
        (Expression<Func<TEntity, TProperty3>> propertyExpression3, TProperty3 value3) property3,
        Expression<Func<TEntity, bool>> where = null);


    /// <summary>
    /// Obtiene la suma de los valores de una columna
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> where = null);

    /// <summary>
    /// Intenta agregar una entidad
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<GenericDatabaseResponse<TEntity>> TryAddAsync(TEntity entity);

    /// <summary>
    /// Intenta agregar una entidad y obtener el primer registro
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    public Task<TEntity> TryAddOrGetFirstAsync(TEntity entity, Expression<Func<TEntity, bool>> where);

}

/// <summary>
/// Interfáz para control de versión de Fila
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public interface IGenericRepositoryRowControl<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntityControlRow
{
    /// <summary>
    /// Actualiza el control de file
    /// </summary>
    /// <param name="where"></param>
    /// <param name="dateTimeRowControl"></param>
    /// <returns></returns>
    Task<int> UpdateRowControlAsync(
        Expression<Func<TEntity, bool>> where = null,
        DateTime? dateTimeRowControl = null
    );

}