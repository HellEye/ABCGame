using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroup", menuName = "Items/Item Group With Targets", order = 2)]
public class ItemGroupWithTargets : ScriptableObject, ISpawnableGroup {
    public List<ItemSO> nonTargets;
    public List<ItemSO> targets;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty) {
        var pickedItems = nonTargets.PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targetItems = targets.PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targetItems);
        return (targetItems, pickedItems);
    }
}