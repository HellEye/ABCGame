using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ExcludeItemGroup {
    public List<ItemSO> items;
    public ColorblindLevel colorblindLevel;
}

[CreateAssetMenu(fileName = "ExcludeItems", menuName = "Items/ExcludeItems", order = 3)]
public class ExcludeItemsSO : ScriptableObject {
    [Header("Exclude items")]
    [Tooltip(
        "If the colorblindness level is set to the same as in the group or higher, only one item from each group will be able to appear")]
    public List<ExcludeItemGroup> excludeItemGroups;

    public IEnumerable<IElement> ExcludeFrom(IEnumerable<IElement> elements, MainMenuSettingsData settings) {
        var list = elements.ToList();
        // Filter exclude groups based on colorblind level
        // Only include groups where settings.ColorblindLevel >= group.colorblindLevel
        var activeGroups = excludeItemGroups.FindAll(group => settings.ColorblindLevel >= group.colorblindLevel);

        if (activeGroups.Count == 0) return new List<IElement>(list);

        var result = new List<IElement>();
        var itemsToExclude = new HashSet<ItemSO>();
        var itemsInList = new List<ItemSO>(10);
        var listItems = list.OfType<ItemSO>().ToHashSet();

        // For each active exclude group
        foreach (var group in activeGroups) {
            if (group.items == null || group.items.Count == 0) continue;

            // Find all items from this group that appear in the list
            foreach (var item in group.items)
                if (listItems.Contains(item))
                    itemsInList.Add(item);

            // If more than one item from this group appears, mark all but the first for exclusion
            if (itemsInList.Count > 1)
                for (var i = 1; i < itemsInList.Count; i++)
                    itemsToExclude.Add(itemsInList[i]);

            itemsInList.Clear();
        }

        // Build result list, excluding marked items
        foreach (var item in list)
            if (item is not ItemSO itemSo || !itemsToExclude.Contains(itemSo))
                result.Add(item);

        return result;
    }
}