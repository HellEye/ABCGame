using System;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

public class ConnectGameManager : MonoBehaviour, IGameManager {
    readonly List<Item> items = new();
    [Inject] ConnectSpawner connectSpawner;
    int currentCorrect;
    [Inject] ConnectDifficulty difficulty;
    [Inject] GameLoader gameLoader;
    [Inject] ItemContainer itemContainer;

    void Start() {
        currentCorrect = 0;
        items.Clear();
        connectSpawner.SpawnItems(itemContainer.GetTargets());
        foreach (var item in items)
            if (item.Draggable != null)
                item.Draggable.OnDropped += HandleItemDrop;

        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }

    public event Action OnGameComplete;
    public void RestartGame() => gameLoader.ReloadCurrentGameplayScene();

    void HandleItemDrop(DropInfo dropInfo) {
        if (dropInfo.dropType == DropType.Correct) {
            currentCorrect++;
            Debug.Log("Dropped correct " + currentCorrect);
            if (currentCorrect >= difficulty.numberOfItems) OnGameComplete?.Invoke();
        }
    }

    public void AddItem(Item item) => items.Add(item);

    public void CheckGameComplete() { }
}