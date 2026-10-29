using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;

public class ConnectGameManager : MonoBehaviour, IGameManager {
    readonly List<Item> items = new();
    [Inject] ConnectSpawner connectSpawner;
    int currentCorrect;
    [Inject] ConnectDifficulty difficulty;
    [Inject] GameLoader gameLoader;
    [Inject] ItemContainer itemContainer;
    [Inject] GameProgressData progressData;

    void Start() {
        currentCorrect = 0;
        items.Clear();
        progressData.Progress = 0;
        var targets = itemContainer.GetTargets().ToList();
        var fakeItems = itemContainer.GetNonTargets();
        progressData.TotalProgress = targets.Count;
        connectSpawner.SpawnItems(targets, fakeItems);
        foreach (var item in items)
            if (targets.Contains(item.data) && item.Draggable != null)
                item.Draggable.OnDropped += HandleItemDrop;

        Debug.Log("Targets: " + string.Join(", ", targets.Select(t => t.DisplayNameSingular)));
        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }

    public event Action OnGameComplete;
    public void RestartGame() => gameLoader.ReloadCurrentGameplayScene();

    void HandleItemDrop(DropInfo dropInfo) {
        if (dropInfo.dropType == DropType.Correct) {
            currentCorrect++;
            progressData.Progress = currentCorrect;
            Debug.Log("Dropped correct " + currentCorrect);
            if (currentCorrect >= difficulty.numberOfItems) OnGameComplete?.Invoke();
        }
    }

    public void AddItem(Item item) => items.Add(item);
}