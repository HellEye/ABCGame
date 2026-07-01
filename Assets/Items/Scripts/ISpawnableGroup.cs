using System.Collections.Generic;

public enum Difficulty {
    Easy,
    Medium,
    Hard
}

public interface ISpawnableGroup {
    Difficulty Difficulty { get; }
    string Title { get; }
    string TargetText { get; }

    (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty,
        MainMenuSettingsData data, ExcludeItemsSO excludeItems = null);
}