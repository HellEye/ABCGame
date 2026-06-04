using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroup", menuName = "Items/Item Group With Targets", order = 2)]
public class ItemGroupWithTargets : ScriptableObject, ISpawnableGroup {
    public List<ItemSO> nonTargets;
    public List<ItemSO> targets;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData settings, ExcludeItemsSO excludeItems) {
        var joined = new List<ItemSO>(nonTargets);
        joined.AddRange(targets);
        var excludedItems = new HashSet<ItemSO>(excludeItems.ExcludeFrom(joined, settings));
        var excludedNonTargets = nonTargets.Where((i)=>!excludedItems.Contains(i)).ToList();
        var excludedTargets = targets.Where(i=>!excludedItems.Contains(i)).ToList();
        var pickedItems = excludedNonTargets.PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targetItems = excludedTargets.PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targetItems);
        return (targetItems, pickedItems);
    }
}