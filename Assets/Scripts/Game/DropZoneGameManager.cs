using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;

public class DropZoneGameManager : MonoBehaviour {
    [SerializeField] DropZone dropZonePrefab;
    readonly List<DropZone> dropZones = new();

    readonly List<Item> items = new();
    [Inject] ISpawnableGroup allItems;

    [Inject] DropZoneGameDifficulty difficulty;

    [Inject] ItemSpawnerManager itemSpawnerManager;


    void Start() {
        var (targets, pickedItems) = allItems.PickItems(difficulty);
        //itemSpawnerManager.TrySpawningItemsPerType(pickedItems);
        itemSpawnerManager.TrySpawningMaxItems(pickedItems);
        itemSpawnerManager.SpawnDropZones(targets);
        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }


    public event Action OnGameComplete;
    public void AddItem(Item item) => items.Add(item);

    public void RemoveItem(Item item) {
        items.Remove(item);
        // If there are no more items left that match the drop zones, the game is complete
        if (!items.Any(i => dropZones.Exists(d => d.target == i.data))) OnGameComplete?.Invoke();
    }

    public void AddDropZone(DropZone newDropZone) => dropZones.Add(newDropZone);

    public void RestartGame() {
        // Clear existing items and drop zones
        foreach (var item in items) Destroy(item.gameObject);
        items.Clear();

        foreach (var dropZone in dropZones) Destroy(dropZone.gameObject);
        dropZones.Clear();

        // Restart the game
        Start();
    }
}