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
    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData settings, ExcludeItemsSO excludeItems) {
        var joined = new List<ItemSO>(nonTargets);
        joined.AddRange(targets);
        var excludedItems = new HashSet<ItemSO>(excludeItems.ExcludeFrom(joined, settings));
        var excludedNonTargets = nonTargets.Where(i => !excludedItems.Contains(i)).ToList();
        var excludedTargets = targets.Where(i => !excludedItems.Contains(i)).ToList();
        var pickedItems = excludedNonTargets.PickRandom(difficulty.itemTypes - difficulty.targetTypes);
        var targetItems = excludedTargets.PickRandom(difficulty.targetTypes);
        pickedItems.AddRange(targetItems);
        return (targetItems, pickedItems);
    }
}