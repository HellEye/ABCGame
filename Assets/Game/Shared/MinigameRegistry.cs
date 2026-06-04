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
    public SceneReference sceneReference;
    public DifficultyMapping[] mappings;
}

[CreateAssetMenu(fileName = "DifficultyRegistry", menuName = "ScriptableObjects/DifficultyRegistry")]
public class MinigameRegistry : ScriptableObject {
    [SerializeField] LevelMapping[] mappings;

    public (SceneReference sceneAsset, IDifficulty<ScriptableObject> difficulty) GetLevelData(int levelIndex,
        int difficultyIndex) {
        var levelMapping = mappings.FirstOrDefault(m => m.levelIndex == levelIndex);
        if (levelMapping.mappings == null || levelMapping.mappings.Length <= difficultyIndex) return (null, null);

        var difficultyMapping = levelMapping.mappings[difficultyIndex];
        if (difficultyMapping.difficultyData == null) return (null, null);

        return (levelMapping.sceneReference, difficultyMapping.difficultyData.Value);
    }
}

public interface IDifficulty<out T> where T : ScriptableObject {
    Type type => typeof(T);
    Difficulty Difficulty { get; }
}


public class DifficultyHolder {
    public IDifficulty<ScriptableObject> selectedDifficulty;
    public SceneReference selectedScene;
}