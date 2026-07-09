using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRegistry", menuName = "ScriptableObjects/Item Registry", order = 1)]
public class ItemRegistry : ScriptableObject {
    public List<InterfaceReference<ISpawnableGroup>> items;
    public List<InterfaceReference<ISpawnableGroup>> letters;

    public IEnumerable<ISpawnableGroup> GetGroupsFor(IDifficulty<ScriptableObject> difficulty) {
        var list = difficulty.Variant switch {
            Variant.Items => items,
            Variant.Letters => letters,
            var _ => throw new NotImplementedException()
        };
        return list.Where(i => i.Value.Difficulty == difficulty.Difficulty).Select(i => i.Value);
    }
}