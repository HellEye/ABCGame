using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroup", menuName = "Items/Group With Targets", order = 2)]
public class ItemGroupWithTargets : ScriptableObject, ISpawnableGroup {
    [SerializeField] string groupName;

    [SerializeField] [Tooltip("Text to display as a hint")]
    string targetText;

    public List<ItemSO> nonTargets;
    public List<ItemSO> targets;
    [SerializeField] Difficulty difficulty;

    [InfoBanner("This group doesn't have enough items that are not placeholders")]

    public bool IsPlaceholder =>
        targets.All(i => i.IsPlaceholder)
        || nonTargets.Count(i => !i.IsPlaceholder) < 3;

    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (IEnumerable<IElement> targets, IEnumerable<IElement> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData settings, ExcludeItemsSO excludeItems) {
        var joined = new List<ItemSO>(nonTargets);
        joined.AddRange(targets);
        var excludedItems = excludeItems.ExcludeFrom(joined, settings).OfType<ItemSO>()
            .Where(item => !Debug.isDebugBuild || item.IsPlaceholder).ToHashSet();
        var excludedNonTargets = nonTargets.Where(i => !excludedItems.Contains(i)).ToList();
        var excludedTargets = targets.Where(i => !excludedItems.Contains(i)).ToList();
        var pickedItems = excludedNonTargets.Where(i => Debug.isDebugBuild || !i.IsPlaceholder)
            .PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targetItems = excludedTargets.Where(i => Debug.isDebugBuild || !i.IsPlaceholder)
            .PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targetItems);
        return (targetItems, pickedItems);
    }
}