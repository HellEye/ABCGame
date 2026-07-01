using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroup", menuName = "Items/Group", order = 2)]
public class ItemGroup : ScriptableObject, ISpawnableGroup {
    [SerializeField] string groupName;

    [SerializeField] [Tooltip("Text to display as a hint")]
    string targetText;

    public List<ItemSO> items;

    [SerializeField] Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData settings, ExcludeItemsSO excludeItems) {
        var excludedItems = excludeItems.ExcludeFrom(items, settings);
        var pickedItems = excludedItems.PickRandom(difficulty.itemTypes);
        var targets = pickedItems.PickRandom(difficulty.targetTypes);
        return (targets, pickedItems);
    }
}