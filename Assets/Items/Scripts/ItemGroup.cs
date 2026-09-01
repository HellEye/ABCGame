using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroup", menuName = "Items/Group", order = 2)]
public class ItemGroup : ScriptableObject, ISpawnableGroup {
    [SerializeField] string groupName;

    [SerializeField] [Tooltip("Text to display as a hint")]
    string targetText;

    public List<ItemSO> items;

    [SerializeField] Difficulty difficulty;

    [InfoBanner("This group doesn't have enough items that are not placeholders")]

    public bool IsPlaceholder => items.Count(item => !item.IsPlaceholder) <= 3;

    public Difficulty Difficulty => difficulty;
    public string Title => groupName;
    public string TargetText => targetText;

    public (IEnumerable<IElement> targets, IEnumerable<IElement> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData settings, ExcludeItemsSO excludeItems) {
        var excludedItems = excludeItems.ExcludeFrom(items, settings).ToList();
        var pickedItems = excludedItems.Where(i => Debug.isDebugBuild || !i.IsPlaceholder)
            .PickRandom(difficulty.itemTypes);
        var targets = pickedItems.PickRandom(difficulty.targetTypes);
        return (targets, pickedItems);
    }
}