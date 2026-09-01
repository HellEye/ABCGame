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

    public bool IsPlaceholder =>
        categories.SelectMany(c => c.items).Count(i => !i.IsPlaceholder) < 3 ||
        targets.All(item => item.IsPlaceholder);

    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (IEnumerable<IElement> targets, IEnumerable<IElement> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData data, ExcludeItemsSO excludedItems) {
        var allItems = categories.SelectMany(c => c.items).Cast<IElement>().ToList();
        var excludedAllItems = excludedItems.ExcludeFrom(allItems, data).ToList();
        var excludedTargets = excludedItems.ExcludeFrom(this.targets, data).ToList();
        var nonTargets = excludedAllItems.Except(excludedTargets).ToList();
        var pickedItems = nonTargets.Where(i => Debug.isDebugBuild || !i.IsPlaceholder)
            .PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targets = excludedTargets.Where(i => Debug.isDebugBuild || !i.IsPlaceholder)
            .PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targets);
        return (targets, pickedItems);
    }

    static string DescribeElement(IElement element) =>
        element is Object unityObject ? unityObject.name : element?.ToString() ?? "null";
}