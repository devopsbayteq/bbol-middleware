namespace Common.WebApi.Extensions;
public static class DictionaryExtension
{
    /// <summary>
    /// Retorna el primer registro encontrado o null
    /// </summary>
    /// <param name="inputDictionary"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TU"></typeparam>
    /// <returns></returns>
    public static TU FirstOrDefaultValue<T, TU>(this IDictionary<T, TU> inputDictionary, Func<KeyValuePair<T, TU>, bool> predicate)
        => inputDictionary.FirstOrDefault(predicate).Equals(default(KeyValuePair<T, TU>)) ? default : inputDictionary.SingleOrDefault(predicate).Value;
}
