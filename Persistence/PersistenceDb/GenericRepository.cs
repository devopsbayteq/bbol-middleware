using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using N.EntityFrameworkCore.Extensions;
using PersistenceDb.CustomException;
using PersistenceDb.Model;
using PersistenceDb.Models.Enums;
using PersistenceDb.Models.Models;
using PersistenceDb.Repository.Interfaces;
using PersistenceDb.Repository.Models;

namespace PersistenceDb;

/// <summary>
/// Constructor
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class GenericRepository<TEntity>(
    PersistenceContext context,
    ILogger<GenericRepository<TEntity>> logger) : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly PersistenceContext Context = context;
    protected readonly ILogger<GenericRepository<TEntity>> Logger = logger;

    /// <summary>
    /// Obtiene entidades 
    /// </summary>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public async Task<List<TEntity>> GetByAsync(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType? orderByType = default,
        int? top = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            var entitySet = Context.Set<TEntity>();
            var query = includes.Aggregate(
                            entitySet.AsQueryable(),
                            (current, include) => current.Include(include)
                        );
            query = query
            .AsNoTracking()
            .Where(where ?? (t => true));
            if (top is not null)
                query = query.Take(top.Value);
            if (orderBy is not null)
                query = orderByType == OrderByType.Asc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción Buscando una lista de Entidades para {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al obtener una lista de entidades para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Agrega una tentidad
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<TEntity> AddAsync(TEntity entity, bool autoDetectChangesEnabled = false)
    {
        try
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            var entityResult = await Context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            _ = await Context.SaveChangesAsync();
            return entityResult.Entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al guardar el nuevo {@Entity}: {@ErrorMessage} {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al agregar una entidad para la entidad {typeof(TEntity).Name}.", ex);
        }
        finally
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    /// <summary>
    /// Actualiza Entidades
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        try
        {
            var updateEntity = Context.Set<TEntity>().Update(entity);
            _ = await Context.SaveChangesAsync();
            return updateEntity.Entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al actualizar el {Entity}:{ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al actualizar una entidad para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Actualiza Entidades
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<int> UpdateRangeAsync(IList<TEntity> entities)
    {
        try
        {
            /// await Context.BulkUpdateAsync(entities).ConfigureAwait(false);
            return await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al actualizar Entidades (Bulk) {@Entity}:{@ErrorMessage} {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al actualizar entidades (Bulk) para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Elimina una Entidad
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(TEntity entity)
    {
        try
        {
            Context.Set<TEntity>().Remove(entity);
            _ = await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al eliminar {Entity}:{ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al eliminar una entidad para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Elimina una Entidad
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where)
    {
        try
        {
            where ??= t => true;
            return (await Context.Set<TEntity>().Where(where).ExecuteDeleteAsync().ConfigureAwait(false)) > 0;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al eliminar Entidades (Bulk) {@Entity}:{@ErrorMessage} {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            if (ex.InnerException is not null)
                Logger.LogError(ex.InnerException, "Inner exception {Entity} : {ErrorMessage}", typeof(TEntity), ex.InnerException);
            throw new CustomPersistenceException($"Error al eliminar entidades para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Obtiene el primer registro mediante un filtro y con entidades Incluidas (JOIN)
    /// </summary>
    /// <param name="where"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public async Task<TEntity> GetByFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> where = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            where ??= t => true;
            var query = Context.Set<TEntity>();
            return await includes
            .Aggregate(
                query.AsQueryable(),
                (current, include) => current.Include(include)
            ).AsNoTracking().FirstOrDefaultAsync(where).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando los items en base al filtro para {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al obtener el primer registro genérico para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Obtiene el primer registro mediante un filtro y con entidades Incluidas (JOIN)
    /// </summary>
    /// <param name="where"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public async Task<TResult> GetFirstOrDefaultGenericAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null)
        => await GetFirstOrDefaultGenericAsync(selector, [where ?? (t => true)]);

    /// <summary>
    /// Obtiene el primer registro mediante una lista de filtros y con entidades Incluidas (JOIN)
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="whereClauses"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<TResult> GetFirstOrDefaultGenericAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        List<Expression<Func<TEntity, bool>>> whereClauses)
    {
        try
        {
            if (whereClauses.Count == 0)
                whereClauses = [where => true];
            var entitySet = Context.Set<TEntity>();
            var originalWhereQuery = entitySet.AsQueryable();
            foreach (var itemWhere in whereClauses)
                originalWhereQuery = originalWhereQuery.Where(itemWhere);
            return await originalWhereQuery.AsNoTracking().Select(selector).FirstOrDefaultAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando los items en base al filtro para {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al obtener el primer registro genérico para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Verifica si existe algún registro en una tabla
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<bool> ExistAnyAsync(Expression<Func<TEntity, bool>> where = null)
    {
        try
        {
            where ??= t => true;
            return await Context.Set<TEntity>().AsNoTracking().AnyAsync(where).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando si existe un tipo los items para {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al verificar si existe un tipo los items para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Obtiene la cantidad de Registros de una Tabla con un filtrado
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> where = null)
    {
        try
        {
            where ??= t => true;
            return await Context.Set<TEntity>().AsNoTracking().CountAsync(where).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando contando los items para {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al contar los items para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Agrega Varias Entidades
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<int> AddRangeAsync(IEnumerable<TEntity> entity, bool includeChildren = false)
    {
        try
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            var count = await DbContextExtensionsAsync.BulkInsertAsync(Context, entity).ConfigureAwait(false);
            await Context.BulkSaveChangesAsync();
            return count;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción agregando varias entidadtes de tipo {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al agregar varias entidades para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Agrega Varias Entidades y optioene también el Id generado
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<List<TEntity>> AddRangeIdentityAsync(List<TEntity> entity)
    {
        try
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            await DbContextExtensionsAsync.BulkInsertAsync(Context, entity).ConfigureAwait(false);
            await DbContextExtensionsAsync.BulkSaveChangesAsync(Context).ConfigureAwait(false);
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción agregando varias Entidades de tipo {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            if (ex.InnerException != null)
                Logger.LogError(ex.InnerException, "Inner exception {Entity} : {ErrorMessage}", typeof(TEntity), ex.InnerException);
            throw new CustomPersistenceException($"Error al agregar varias entidades para la entidad {typeof(TEntity).Name}.", ex);
        }
    }


    /// <summary>
    /// Obtiene el valor máximo de una tabla
    /// </summary>
    /// <param name="selector"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<TResult> GetMaxValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        try
        {
            return await Context.Set<TEntity>().AsNoTracking().MaxAsync(selector).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando el valor máximo de una Tabla: {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al obtener el valor máximo de una tabla para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Obtiene el valor Mínimo de una tabla
    /// </summary>
    /// <param name="selector"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<TResult> GetMinValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        try
        {
            return await Context.Set<TEntity>().AsNoTracking().MinAsync(selector).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando el valor máximo de una Tabla: {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al obtener el valor mínimo de una tabla para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Obtiene los resultados Paginados
    /// </summary>
    /// <param name="itemsByPage"></param>
    /// <param name="page">Debe comenzar con 1</param>
    /// <param name="where"></param>
    /// <param name="includes"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<IPaginator<TEntity>> GetPaginatorByAsync(
        int itemsByPage,
        int page,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType orderByType = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            page--;
            where ??= t => true;
            var entitySet = Context.Set<TEntity>();
            var query = includes.Aggregate(
                            entitySet.AsQueryable(),
                            (current, include) => current.Include(include));
            query = query.Where(where);
            if (orderBy is not null)
                query = orderByType == OrderByType.Asc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            query = query.Skip(itemsByPage * page)
            .Take(itemsByPage);
            var items = await query.AsNoTracking().ToListAsync().ConfigureAwait(false);
            var totalItems = await CountAsync(where).ConfigureAwait(false);
            return new PaginatorModel<TEntity>(items, totalItems);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando los items paginados para {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            throw new CustomPersistenceException($"Error al obtener registros paginados para la entidad {typeof(TEntity).Name}.", ex);
        }
    }


    /// <summary>
    /// Obtiene un Objeto Genérico
    /// /// </summary>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderByType"></param>
    /// <param name="includes"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<List<TResult>> GetGenericAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType orderByType = default)
    {
        try
        {
            where ??= t => true;
            var entitySet = Context.Set<TEntity>();
            var query = entitySet.Where(where);
            if (orderBy is not null)
                query = orderByType == OrderByType.Asc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.AsNoTracking().Select(selector).ToListAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando genéricos los items en base al filtro para {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al obtener registros genéricos para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Obtiene un Objeto genérico Paginado
    /// </summary>
    /// <param name="itemsByPage"></param>
    /// <param name="page"></param>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderByType"></param>
    /// <param name="includes"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<IPaginator<TResult>> GetPaginatorGenericAsync<TResult>(
        int itemsByPage,
        int page,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType orderByType = default)
        => await GetPaginatorGenericAsync(itemsByPage, page, selector, [where ?? (t => true)], orderBy, orderByType);


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
    public async Task<IPaginator<TResult>> GetPaginatorGenericAsync<TResult>(
        int itemsByPage,
        int page,
        Expression<Func<TEntity, TResult>> selector,
        List<Expression<Func<TEntity, bool>>> where,
        Expression<Func<TEntity, dynamic>> orderBy = null,
        OrderByType orderByType = default)
    {
        try
        {
            page--;
            if (where.Count == 0)
                where = [t => true];
            var entitySet = Context.Set<TEntity>();
            var originalWhereQuery = entitySet.AsQueryable();
            foreach (var itemWhere in where)
                originalWhereQuery = originalWhereQuery.Where(itemWhere);
            var totalItems = await originalWhereQuery
                .AsNoTracking()
                .CountAsync()
                .ConfigureAwait(false);
            if (orderBy is not null)
                originalWhereQuery = orderByType == OrderByType.Asc ? originalWhereQuery.OrderBy(orderBy) : originalWhereQuery.OrderByDescending(orderBy);
            var items = await originalWhereQuery
                .Skip(itemsByPage * page)
                .Take(itemsByPage)
                .AsNoTracking()
                .Select(selector)
                .ToListAsync()
                .ConfigureAwait(false);
            return new PaginatorModel<TResult>(items, totalItems);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando los items en base al filtro para {Entity}: {ErrorMessage} - {InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al obtener registros distintos para la entidad {typeof(TEntity).Name}.", ex);
        }
    }


    /// <summary>
    /// Actualización directa 
    /// </summary>
    /// <param name="parameterUpdate"></param>
    /// <param name="valueUpdate"></param>
    /// <param name="where"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<List<TResult>> DistinctAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> where = null)
    {
        try
        {
            where ??= t => true;
            var entitySet = Context.Set<TEntity>();
            return await entitySet.Where(where).Select(selector).Distinct().ToListAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando los items distintos en base al filtro para {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al actualizar varios registros para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Actualiza varios registros
    /// </summary>
    /// <param name="property"></param>
    /// <param name="property2"></param>
    /// <param name="property3"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<int> UpdateByAsync<TProperty, TProperty2, TProperty3>(
        (Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) property,
        (Expression<Func<TEntity, TProperty2>> propertyExpression2, TProperty2 value2) property2,
        (Expression<Func<TEntity, TProperty3>> propertyExpression3, TProperty3 value3) property3,
        Expression<Func<TEntity, bool>> where = null)
    {
        try
        {
            where ??= t => true;
            return await Context.Set<TEntity>()
            .Where(where)
            .ExecuteUpdateAsync(set =>
             set.SetProperty(property.propertyExpression, property.value)
                .SetProperty(property2.propertyExpression2, property2.value2)
                .SetProperty(property3.propertyExpression3, property3.value3)).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción actualizando varios registros en base al filtro para {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al actualizar varios registros para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Actualiza varios registros
    /// </summary>
    /// <param name="property"></param>
    /// <param name="property2"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<int> UpdateByAsync<TProperty, TProperty2>(
        (Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) property,
        (Expression<Func<TEntity, TProperty2>> propertyExpression2, TProperty2 value2) property2,
        Expression<Func<TEntity, bool>> where = null)
    {
        try
        {
            where ??= t => true;
            return await Context.Set<TEntity>()
            .Where(where)
            .ExecuteUpdateAsync(set =>
             set.SetProperty(property.propertyExpression, property.value)
                .SetProperty(property2.propertyExpression2, property2.value2)
                ).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción actualizando un registro en base al filtro para {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al actualizar un registro para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Actualiza un registro
    /// </summary>
    /// <param name="propertyExpression"></param>
    /// <param name="value"></param>
    /// <param name="where"></param>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    public async Task<int> UpdateByAsync<TProperty>(
       (Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value) properties,
       Expression<Func<TEntity, bool>> where = null
   )
    {
        where ??= t => true;
        return await Context.Set<TEntity>()
        .Where(where)
        .ExecuteUpdateAsync(set => set.SetProperty(properties.propertyExpression, properties.value)).ConfigureAwait(false);
    }

    /// <summary>
    /// Obtiene la suma de los valores de una columna
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="where"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> where = null)
    {
        try
        {
            return await Context.Set<TEntity>().Where(where).SumAsync(selector).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción buscando la suma de los valores de una columna para {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al obtener la suma de los valores de una columna para la entidad {typeof(TEntity).Name}.", ex);
        }
    }

    /// <summary>
    /// Agrega una tentidad
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<GenericDatabaseResponse<TEntity>> TryAddAsync(TEntity entity)
    {
        try
        {
            var entityResult = await Context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            _ = await Context.SaveChangesAsync();
            return new(entityResult.Entity);
        }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2627 })
        {
            Logger.LogWarning(ex, "Error al guardar el nuevo {Entity}: {ErrorMessage} por llave duplicada.", typeof(TEntity), ex.InnerException?.Message);
            return new GenericDatabaseResponse<TEntity>(ErrorDatabaseType.PrimaryKeyDuplicate);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al guardar el nuevo {Entity}: {ErrorMessage}", typeof(TEntity), ex.Message);
            if (ex.InnerException != null)
                Logger.LogError(ex.InnerException, "Inner exception {Entity} : {ErrorMessage}", typeof(TEntity), ex.InnerException);
            return new GenericDatabaseResponse<TEntity>(ErrorDatabaseType.ErrorNotMapped);
        }
    }

    /// <summary>
    /// Agrega una tentidad
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<TEntity> TryAddOrGetFirstAsync(TEntity entity, Expression<Func<TEntity, bool>> where)
    {
        try
        {
            var entityResult = await Context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            _ = await Context.SaveChangesAsync();
            return entityResult.Entity;
        }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2627 })
        {
            Logger.LogWarning(ex, "Error al guardar el nuevo {Entity}: {ErrorMessage} por llave duplicada.", typeof(TEntity), ex.InnerException?.Message);
            return await GetByFirstOrDefaultAsync(where).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción Intentando hacer un Insert o Get {@Entity}: {@ErrorMessage} - {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al intentar hacer un Insert o Get para la entidad {typeof(TEntity).Name}.", ex);
        }
    }
}
/// <summary>
/// Repositorio Genérico para Control de Fila
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class GenericRepositoryRowControl<TEntity>(PersistenceContext context, ILogger<GenericRepository<TEntity>> logger)
    : GenericRepository<TEntity>(context, logger), IGenericRepositoryRowControl<TEntity> where TEntity : class, IEntityControlRow
{
    /// <summary>
    /// Actualiza el control de Filas
    /// </summary>
    /// <param name="where"></param>
    /// <param name="dateTimeRowControl"></param>
    /// <returns></returns>
    public async Task<int> UpdateRowControlAsync(
        Expression<Func<TEntity, bool>> where = null,
        DateTime? dateTimeRowControl = null)
    {
        try
        {
            dateTimeRowControl ??= DateTime.UtcNow;
            where ??= t => true;
            return await Context.Set<TEntity>()
                .Where(where)
                .ExecuteUpdateAsync(update => update.SetProperty(row => row.RowControl, dateTimeRowControl));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exepción actualizando el control de Filas para {@Entity}: {@ErrorMessage} {@InnerException}", typeof(TEntity), ex.Message, ex.InnerException?.Message);
            throw new CustomPersistenceException($"Error al actualizar el control de Filas para la entidad {typeof(TEntity).Name}.", ex);
        }
    }
}