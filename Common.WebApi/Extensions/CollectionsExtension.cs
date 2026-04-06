namespace Common.Utils.Extensions;

public static class CollectionsExtension
{
    /// <summary>
    /// Verifica si la lista es null o está vacía
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
    => list is null || (!list.Any());

}
