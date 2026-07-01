using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Items/Category", order = 3)]
public class ItemCategorySO : ScriptableObject, ISpawnableGroup {
    [SerializeField] string groupName;

    [SerializeField] [Tooltip("Text to display as a hint")]
    string targetText;

    public List<ItemSO> items;
    [SerializeField] Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData data, ExcludeItemsSO excludedItems) {
        var excludedList = excludedItems.ExcludeFrom(items, data);
        var pickedItems = excludedList.PickRandom(difficulty.itemTypes);
        var targets = pickedItems.PickRandom(difficulty.targetTypes);
        return (targets, pickedItems);
    }
}