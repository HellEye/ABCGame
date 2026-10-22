using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;

public class DropZoneGameManager : MonoBehaviour, IGameManager {
    [SerializeField] DropZone dropZonePrefab;

    readonly List<Item> items = new();
    DropZone dropZone;
    [Inject] GameLoader gameLoader;

    [Inject] ItemContainer itemContainer;

    [Inject] ItemSpawnerManager itemSpawnerManager;

    void Start() {
        //itemSpawnerManager.TrySpawningItemsPerType(pickedItems);
        itemSpawnerManager.TrySpawningMaxItems(itemContainer.items);
        dropZone = itemSpawnerManager.SpawnDropZone(itemContainer.targets);
        OnGameComplete += () => Debug.Log("Game Complete!!!");
        foreach (var item in items)
            if (item.Draggable != null)
                item.Draggable.OnDropped += HandleItemDrop;
    }


    public event Action OnGameComplete;

    public void RestartGame() => gameLoader.ReloadCurrentGameplayScene();

    void HandleItemDrop(DropInfo dropInfo) {
        if (dropInfo.dropType == DropType.Correct) RemoveItem(dropInfo.item);
    }

    public void AddItem(Item item) => items.Add(item);

    public void RemoveItem(Item item) {
        items.Remove(item);
        // If there are no more items left that match the drop zone, the game is complete
        if (!items.Any(i => dropZone.targets.Contains(i.data))) OnGameComplete?.Invoke();
    }
}