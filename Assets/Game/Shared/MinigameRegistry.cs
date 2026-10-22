using System;
using System.Linq;
using Eflatun.SceneReference;
using Unity.Properties;
using UnityEngine;

[Serializable]
public struct DifficultyMapping {
    // can add different data here later if necessary
    public InterfaceReference<IDifficulty> difficultyData;

    public bool IsValid(ItemRegistry itemRegistry) => itemRegistry.GetGroupsFor(difficultyData.Value).Any();
}

[Serializable]
public class LevelMapping {
    [CreateProperty] public string levelName;

    [CreateProperty] public Sprite levelIcon;

    public SceneReference sceneReference;
    public DifficultyMapping[] difficultiesMappings;
    public bool WIP;
}

[CreateAssetMenu(fileName = "MinigameRegistry", menuName = "ScriptableObjects/MinigameRegistry")]
public class MinigameRegistry : ScriptableObject {
    [SerializeField] LevelMapping[] mappings;

    public LevelMapping[] Mappings => mappings;

    public int Count => mappings.Length;

    public LevelMapping[] GetMappings() => mappings;
}

public interface IDifficulty {
    Type type { get; }
    Difficulty Difficulty { get; }
    Variant Variant { get; }
    string Name { get; }
    int ItemTypes { get; }
    int TargetTypes { get; }
}

public enum Variant {
    Items,
    Letters
}

public class DifficultyHolder {
    public IDifficulty selectedDifficulty;
    public LevelMapping selectedMapping;
    public SceneReference selectedScene;
}