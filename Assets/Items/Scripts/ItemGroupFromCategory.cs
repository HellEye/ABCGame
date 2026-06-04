using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Items/Group from category", order = 3)]
public class ItemGroupFromCategory : ScriptableObject, ISpawnableGroup {
    [SerializeField] string groupName;

    [SerializeField] [Tooltip("Text to display as a hint")]
    string targetText;

    public List<ItemSO> targets;
    public List<ItemCategorySO> categories;
    [SerializeField] Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData data, ExcludeItemsSO excludedItems) {
        var allItems = categories.SelectMany(c => c.items).ToList();
        var excludedAllItems = excludedItems.ExcludeFrom(allItems, data);
        var excludedTargets = excludedItems.ExcludeFrom(this.targets, data);
        var nonTargets = excludedAllItems.Except(excludedTargets).ToList();
        var pickedItems = nonTargets.PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targets = excludedTargets.PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targets);
        Debug.Log($"Picked {pickedItems.Count} items and {targets.Count} targets");
        Debug.Log($"Picked items: {string.Join(", ", pickedItems.Select(i => i.name))}");
        Debug.Log($"Picked targets: {string.Join(", ", targets.Select(i => i.name))}");
        return (targets, pickedItems);
    }
}