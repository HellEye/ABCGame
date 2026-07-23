using System;
using System.Linq;
using Eflatun.SceneReference;
using UnityEngine;

[Serializable]
public struct DifficultyMapping {
    // can add different data here later if necessary
    public InterfaceReference<IDifficulty<ScriptableObject>> difficultyData;
}

[Serializable]
public struct LevelMapping {
    public int levelIndex;
    public string levelName;
    public Sprite levelIcon;
    public SceneReference sceneReference;
    public DifficultyMapping[] difficultiesMappings;
}

[CreateAssetMenu(fileName = "MinigameRegistry", menuName = "ScriptableObjects/MinigameRegistry")]
public class MinigameRegistry : ScriptableObject {
    [SerializeField] private LevelMapping[] mappings;
    public LevelMapping[] GetMapping
    {
        get => mappings;
    }
    public int Count => mappings.Length;

    public (SceneReference sceneAsset, IDifficulty<ScriptableObject> difficulty) GetLevelData(int levelIndex,
        int difficultyIndex) {
        var levelMapping = mappings.FirstOrDefault(m => m.levelIndex == levelIndex);
        var difficultyMapping =
            levelMapping.difficultiesMappings.Where(d => d.difficultyData.Value.Difficulty == (Difficulty)difficultyIndex)
                .PickRandom();

        if (difficultyMapping.difficultyData == null) return (null, null);

        return (levelMapping.sceneReference, difficultyMapping.difficultyData.Value);
    }

    public LevelMapping[] GetMappings()
    {
        return mappings;
    }
}

public interface IDifficulty<out T> where T : ScriptableObject {
    Type type => typeof(T);
    Difficulty Difficulty { get; }
    Variant Variant { get; }
}

public enum Variant {
    Items,
    Letters
}

public class DifficultyHolder {
    public IDifficulty<ScriptableObject> selectedDifficulty;
    public SceneReference selectedScene;
}