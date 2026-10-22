using System;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

public class ConnectGameManager : MonoBehaviour, IGameManager {
    readonly List<Item> items = new();
    [Inject] ConnectSpawner connectSpawner;
    [Inject] GameLoader gameLoader;
    [Inject] ItemContainer itemContainer;

    void Start() {
        connectSpawner.SpawnItems(itemContainer.items);
        items.Clear();

        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }

    public event Action OnGameComplete;
    public void RestartGame() => gameLoader.ReloadCurrentGameplayScene();

    public void AddItem(Item item) => items.Add(item);

    public void CheckGameComplete() { }
}