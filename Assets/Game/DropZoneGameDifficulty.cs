using UnityEngine;

[CreateAssetMenu(fileName = "DropZoneGameDifficulty", menuName = "ScriptableObjects/DropZoneGameDifficulty")]
public class DropZoneGameDifficulty : ScriptableObject, IDifficulty<DropZoneGameDifficulty> {
    [Header("Difficulty settings")]
    [SerializeField] private string difficultyName = "Default";
    
    public Variant variant;
    public Difficulty difficulty = Difficulty.Easy;
    public int itemTypes = 3;
    public int targetTypes = 1;
    
    public string DifficultyName => difficulty.ToString();
    
    [Header("Difficulty sprites")]
    [SerializeField] private Sprite difficultyIcon;
    public Sprite DifficultyIcon => difficultyIcon;

    [Header("Spawn settings")]
    public int maxItems = 10;

    public int itemsPerType = 3;
    public Difficulty Difficulty => difficulty;
    public Variant Variant => variant;
}