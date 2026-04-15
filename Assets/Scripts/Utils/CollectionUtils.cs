using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class CollectionUtils {
    public static List<T> PickRandom<T>(
        this IReadOnlyList<T> items,
        int count) {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        if (count < 0 || count > items.Count)
            throw new ArgumentOutOfRangeException(nameof(count));

        var indices = new int[items.Count];
        for (var i = 0; i < indices.Length; i++) indices[i] = i;

        var result = new List<T>(count);

        for (var i = 0; i < count; i++) {
            var j = Random.Range(i, items.Count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
            result.Add(items[indices[i]]);
        }

        return result;
    }
}