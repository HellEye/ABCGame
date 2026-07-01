using System.Collections.Generic;

/// <summary>
///     Used to store the randomly picked items and targets for the drop zones, for consistent access
/// </summary>
public class DropZoneItems {
    public readonly List<ItemSO> items;
    public readonly List<ItemSO> targets;

    public DropZoneItems(ISpawnableGroup spawnableGroup, DropZoneGameDifficulty difficulty,
        ExcludeItemsSO excludeItems, MainMenuSettingsData settings) =>
        (targets, items) = spawnableGroup.PickItems(difficulty, settings, excludeItems);
}