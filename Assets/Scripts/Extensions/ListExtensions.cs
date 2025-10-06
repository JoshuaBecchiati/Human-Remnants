using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static List<int> FindAllIndexes<T>(this List<T> list, Func<T, bool> predicate)
    {
        return list
            .Select((item, index) => new { item, index })
            .Where(x => predicate(x.item))
            .Select(x => x.index)
            .ToList();
    }
}
