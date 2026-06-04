using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroup", menuName = "Items/Item Group", order = 2)]
public class ItemGroup : ScriptableObject, ISpawnableGroup {
    public List<ItemSO> items;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData settings, ExcludeItemsSO excludeItems) {
        var excludedItems = excludeItems.ExcludeFrom(items, settings);
        var pickedItems = excludedItems.PickRandom(difficulty.itemTypes);
        var targets = pickedItems.PickRandom(difficulty.targetTypes);
        return (targets, pickedItems);
    }
}