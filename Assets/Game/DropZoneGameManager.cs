using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;

public class DropZoneGameManager : MonoBehaviour, IGameManager {
    [SerializeField] DropZone dropZonePrefab;

    readonly List<Item> items = new();
    DropZone dropZone;

    [Inject] DropZoneItems dropZoneItems;
    [Inject] GameLoader gameLoader;

    [Inject] ItemSpawnerManager itemSpawnerManager;

    void Start() {
        //itemSpawnerManager.TrySpawningItemsPerType(pickedItems);
        itemSpawnerManager.TrySpawningMaxItems(dropZoneItems.items);
        dropZone = itemSpawnerManager.SpawnDropZone(dropZoneItems.targets);
        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }


    public event Action OnGameComplete;

    public void RestartGame() => gameLoader.ReloadCurrentGameplayScene();

    public void AddItem(Item item) => items.Add(item);

    public void RemoveItem(Item item) {
        items.Remove(item);
        // If there are no more items left that match the drop zone, the game is complete
        if (!items.Any(i => dropZone.targets.Contains(i.data))) OnGameComplete?.Invoke();
    }
}