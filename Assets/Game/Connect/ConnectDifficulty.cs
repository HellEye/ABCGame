using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ConnectDifficulty", menuName = "ScriptableObjects/ConnectDifficulty")]
public class ConnectDifficulty : ScriptableObject, IDifficulty {
    [Header("Difficulty settings")]
    [SerializeField] string difficultyName = "Default";

    public Variant variant;
    public Difficulty difficulty = Difficulty.Easy;

    [Header("Game settings")]
    public int numberOfItems = 2;

    [Tooltip("Unused for now")] public int unconnectedItems;

    public Difficulty Difficulty => difficulty;
    public Variant Variant => variant;
    public Type type => typeof(ConnectDifficulty);
    public string Name => difficultyName;
    public int ItemTypes => numberOfItems + unconnectedItems * 2;
    public int TargetTypes => numberOfItems;
}