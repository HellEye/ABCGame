using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Items/Category", order = 3)]
public class ItemCategorySO : ScriptableObject, ISpawnableGroup {
    public string title;
    public List<ItemSO> items;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty) {
        var pickedItems = items.PickRandom(difficulty.itemTypes);
        var targets = pickedItems.PickRandom(difficulty.targetTypes);
        return (targets, pickedItems);
    }
}