using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRegistry", menuName = "ScriptableObjects/Item Registry", order = 1)]
public class ItemRegistry : ScriptableObject {
    public List<InterfaceReference<ISpawnableGroup>> items;

    public IEnumerable<ISpawnableGroup> GetGroupsFor(Difficulty difficulty) =>
        items.Where(i => i.Value.Difficulty == difficulty).Select(i => i.Value);
}