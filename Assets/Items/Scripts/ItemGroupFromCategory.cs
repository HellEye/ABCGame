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

    public (IEnumerable<IElement> targets, IEnumerable<IElement> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData data, ExcludeItemsSO excludedItems) {
        var allItems = categories.SelectMany(c => c.items).Cast<IElement>().ToList();
        var excludedAllItems = excludedItems.ExcludeFrom(allItems, data).ToList();
        var excludedTargets = excludedItems.ExcludeFrom(this.targets, data).ToList();
        var nonTargets = excludedAllItems.Except(excludedTargets).ToList();
        var pickedItems = nonTargets.PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targets = excludedTargets.PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targets);
        Debug.Log($"Picked {pickedItems.Count} items and {targets.Count} targets");
        Debug.Log($"Picked items: {string.Join(", ", pickedItems.Select(DescribeElement))}");
        Debug.Log($"Picked targets: {string.Join(", ", targets.Select(DescribeElement))}");
        return (targets, pickedItems);
    }

    static string DescribeElement(IElement element) =>
        element is Object unityObject ? unityObject.name : element?.ToString() ?? "null";
}