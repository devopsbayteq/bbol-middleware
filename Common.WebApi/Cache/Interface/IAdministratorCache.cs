namespace Common.WebApi.Cache.Interface;
public interface IAdministratorCache
{

    /// <summary>
    /// Configura un valor en el Cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="seconds"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SetAsync<T>(
        string key,
        T value,
        int seconds = 3600,
        bool slidingExpiration = false);

    /// <summary>
    /// Obtiene un valor del Cache
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> TryGetAsync<T>(string key);

    /// <summary>
    /// Elimina un valor del cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Verifica si existe un key registrado
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> ExistKeyAsync(string key);

    /// <summary>
    /// Obtiene el valor o lo asigna mediante un callback
    /// </summary>
    /// <param name="key"></param>
    /// <param name="callbackAsync"></param>
    /// <param name="minutes"></param>
    /// <param name="slidingExpiration"></param>
    /// <param name="throwExceptionIfNotFound"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> TryGetOrSetAsync<T>(
        string key,
        Func<Task<T>> callbackAsync,
        int seconds = 3600,
        bool slidingExpiration = false,
        bool throwExceptionIfNotFound = true);
}
