using System;
using Eflatun.SceneReference;
using Unity.Properties;
using UnityEngine;

[Serializable]
public struct DifficultyMapping {
    // can add different data here later if necessary
    public InterfaceReference<IDifficulty<ScriptableObject>> difficultyData;
}

[Serializable]
public class LevelMapping {
    [CreateProperty] public string levelName;

    [CreateProperty] public Sprite levelIcon;

    public SceneReference sceneReference;
    public DifficultyMapping[] difficultiesMappings;
}

[CreateAssetMenu(fileName = "MinigameRegistry", menuName = "ScriptableObjects/MinigameRegistry")]
public class MinigameRegistry : ScriptableObject {
    [SerializeField] LevelMapping[] mappings;

    public LevelMapping[] Mappings => mappings;

    public int Count => mappings.Length;

    /*
    [Obsolete ("Kod pisany na szybko do zmiany lub wyrzucenia")]
    public (SceneReference sceneAsset, IDifficulty<ScriptableObject> difficulty) GetLevelData(int levelIndex,
        int difficultyIndex) {
        var levelMapping = mappings.FirstOrDefault(m => m.levelIndex == levelIndex);
        var difficultyMapping =


                .PickRandom();

        if (difficultyMapping.difficultyData == null) return (null, null);

        return (levelMapping.sceneReference, difficultyMapping.difficultyData.Value);
    }
    */

    public LevelMapping[] GetMappings() => mappings;
}

public interface IDifficulty<out T> where T : ScriptableObject {
    Type type => typeof(T);
    Difficulty Difficulty { get; }
    Variant Variant { get; }
    string Name { get; }
}

public enum Variant {
    Items,
    Letters
}

public class DifficultyHolder {
    public IDifficulty<ScriptableObject> selectedDifficulty;
    public SceneReference selectedScene;
}