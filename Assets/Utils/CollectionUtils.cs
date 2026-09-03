using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class CollectionUtils {
    public static List<T> PickRandom<T>(
        this IEnumerable<T> items,
        int count) {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        var enumerable = items as T[] ?? items.ToArray();
        if (count < 0 || count > enumerable.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        var indices = new int[enumerable.Length];
        for (var i = 0; i < indices.Length; i++) indices[i] = i;

        var result = new List<T>(count);

        for (var i = 0; i < count; i++) {
            var j = Random.Range(i, enumerable.Length);
            (indices[i], indices[j]) = (indices[j], indices[i]);
            result.Add(enumerable[indices[i]]);
        }

        return result;
    }

    public static T PickRandom<T>(this IEnumerable<T> items) {
        var count = items.Count();
        var random = Random.Range(0, count);
        return items.ElementAt(random);
    }

    public static IEnumerable<(T, int)> Indexed<T>(this IEnumerable<T> items) {
        var i = 0;
        foreach (var item in items) yield return (item, i++);
    }
}