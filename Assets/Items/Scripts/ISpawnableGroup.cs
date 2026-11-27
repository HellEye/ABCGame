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

    bool IsPlaceholder { get; }

    (IEnumerable<IElement> targets, IEnumerable<IElement> allItems) PickItems(IDifficulty difficulty,
        MainMenuSettingsData data, ExcludeItemsSO excludeItems = null);
}