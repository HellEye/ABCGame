using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DropZoneGameDifficulty", menuName = "ScriptableObjects/DropZoneGameDifficulty")]
public class DropZoneGameDifficulty : ScriptableObject, IDifficulty {
    [Header("Difficulty settings")]
    [SerializeField] string difficultyName = "Default";

    public Variant variant;
    public Difficulty difficulty = Difficulty.Easy;
    public int itemTypes = 3;
    public int targetTypes = 1;

    [Header("Difficulty sprites")]
    [SerializeField] Sprite difficultyIcon;

    [Header("Spawn settings")]
    public int maxItems = 10;

    public int itemsPerType = 3;
    public Sprite DifficultyIcon => difficultyIcon;
    public Difficulty Difficulty => difficulty;
    public Variant Variant => variant;
    public Type type => typeof(DropZoneGameDifficulty);
    public string Name => difficultyName;
    public int ItemTypes => itemTypes;
    public int TargetTypes => targetTypes;
}