using UnityEngine;

[CreateAssetMenu(fileName = "DropZoneGameDifficulty", menuName = "Scriptable Objects/DropZoneGameDifficulty")]
public class DropZoneGameDifficulty : ScriptableObject {
    [Header("Difficulty settings")]
    public string difficultyName = "Default";

    public int itemTypes = 3;
    public int targetTypes = 1;

    [Header("Spawn settings")]
    public int maxItems = 10;

    public int itemsPerType = 3;
}