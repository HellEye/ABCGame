using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BackgroundStore {
    readonly Dictionary<LevelMapping, AssetReferenceSprite> previouslyUsedBg = new();

    public AsyncOperationHandle<Sprite>
        LoadNextBackground(List<AssetReferenceSprite> backgrounds, LevelMapping currentMapping) {
        if (!previouslyUsedBg.TryGetValue(currentMapping, out var bg)) {
            // If this is the first run, pick any background
            var nextBgAny = backgrounds.PickRandom();
            previouslyUsedBg.Add(currentMapping, nextBgAny);
            return nextBgAny.Load();
        }

        // Otherwise, skip the previous background
        var nextBg = backgrounds
            // IDK why I need to do string comparison, it should work by reference.
            .Where(b => b.SubObjectName != bg.SubObjectName)
            .PickRandom();
        previouslyUsedBg[currentMapping] = nextBg;
        return nextBg.Load();
    }

    public AssetReferenceSprite GetCurrentBg(LevelMapping mapping) => previouslyUsedBg[mapping];
}