using System.Collections.Generic;
using System.Linq;

/// <summary>
///     Used to store the randomly picked items and targets for the drop zones, for consistent access
/// </summary>
public class DropZoneItems : IRandomItemContainer {
    public readonly List<ItemSO> items;
    public readonly List<ItemSO> targets;

    public DropZoneItems(ISpawnableGroup spawnableGroup, DropZoneGameDifficulty difficulty,
        ExcludeItemsSO excludeItems, MainMenuSettingsData settings) =>
        (targets, items) = spawnableGroup.PickItems(difficulty, settings, excludeItems);

    public IEnumerable<ItemSO> GetAllItems() => items.Concat(targets);
}