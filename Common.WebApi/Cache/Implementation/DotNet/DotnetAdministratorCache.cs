using Common.WebApi.Cache.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Cache.Implementation.DotNet;
/// <summary>
/// Constructor
/// </summary>
/// <param name="cacheConfigurationModel"></param>
/// <param name="memoryCache"></param>
/// <returns></returns>
public class DotNetAdministratorCache(
    IMemoryCache memoryCache,
    ILogger<DotNetAdministratorCache> logger) : IAdministratorCache
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    /// <summary>
    /// Elimina un dato cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string key)
    => await Task.Run(() => _memoryCache.Remove(key));

    /// <summary>
    ///  Configura un dato en cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="minutes"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task SetAsync<T>(string key, T value, int seconds = 3600, bool slidingExpiration = false)
        => _ = await Task.Run(() =>
            {
                var options = new MemoryCacheEntryOptions();
                _ = slidingExpiration ? options.SetSlidingExpiration(TimeSpan.FromSeconds(seconds)) : options.SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds));
                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("Cache Registrado: {@Key} - Tiempo: {@Seconds} segundos - Tipo : {@Type}", key, seconds, slidingExpiration ? "SetSlidingExpiration" : "SetAbsoluteExpiration");
                return _memoryCache.Set(key, value, options);
            });

    /// <summary>
    /// Obtiene un dato cache
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> TryGetAsync<T>(string key)
    => await Task.Run(() =>
    {
        _memoryCache.TryGetValue(key, out T result);
        return result;
    });

    /// <summary>
    /// Verifica si existe un Key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<bool> ExistKeyAsync(string key)
        => Task.FromResult(_memoryCache.TryGetValue(key, out var _));


    /// <summary>
    /// Obtiene un valor de cache o lo injecta si no lo encuentra
    /// </summary>
    /// <param name="key"></param>
    /// <param name="callbackAsync"></param>
    /// <param name="minutes"></param>
    /// <param name="throwExceptionIfNotFound"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> TryGetOrSetAsync<T>(
        string key,
        Func<Task<T>> callbackAsync,
        int seconds = 3600,
        bool slidingExpiration = false,
        bool throwExceptionIfNotFound = true)
    {
        return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
        {
            _ = slidingExpiration ? cacheEntry.SetSlidingExpiration(TimeSpan.FromSeconds(seconds)) : cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds));
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("Cache Registrado: {@Key} - Tiempo: {@Seconds} segundos - Tipo : {@Type}", key, seconds, slidingExpiration ? "SetSlidingExpiration" : "SetAbsoluteExpiration");
            var result = await callbackAsync().ConfigureAwait(false);
            if (result is null && throwExceptionIfNotFound)
                throw new InvalidOperationException($"No de pudo encontrar en cache ni asignar valores a la Key: '{key}'");
            return result;
        }).ConfigureAwait(false);
    }
}
