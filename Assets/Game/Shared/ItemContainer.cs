using System.Collections.Generic;
using System.Linq;

/// <summary>
///     Used to store the randomly picked items and targets for the drop zones, for consistent access
/// </summary>
public class ItemContainer : IRandomItemContainer {
    public readonly IEnumerable<IElement> items;
    public readonly IEnumerable<IElement> targets;

    public ItemContainer(ISpawnableGroup spawnableGroup, IDifficulty difficulty,
        ExcludeItemsSO excludeItems, MainMenuSettingsData settings) =>
        (targets, items) = spawnableGroup.PickItems(difficulty, settings, excludeItems);

    public IEnumerable<IElement> GetAllItems() => items;
    public IEnumerable<IElement> GetTargets() => targets;
    public IEnumerable<IElement> GetNonTargets() => items.Except(targets);
}