using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DropZoneGameManager : MonoBehaviour
{
    [SerializeField] List<ItemSO> allItems;

    [SerializeField] DropZone dropZonePrefab;

    [SerializeField] ItemSpawnerManager itemSpawnerManager;

    [SerializeField] int itemTypes = 3;
    [SerializeField] int targetTypes = 1;

    readonly List<DropZone> dropZones = new();

    readonly List<Item> items = new();

    // This is allowed, but I guess resharper didn't get the memo
    // ReSharper disable once Unity.IncorrectMethodSignature
    async UniTaskVoid Start()
    {
        var pickedItems = allItems.PickRandom(itemTypes);
        //itemSpawnerManager.TrySpawningItemsPerType(pickedItems);
        await itemSpawnerManager.TrySpawningMaxItems(pickedItems);
        var targets = pickedItems.PickRandom(targetTypes);
        await itemSpawnerManager.SpawnDropZones(targets);
        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }


    public event Action OnGameComplete;
    public void AddItem(Item item) => items.Add(item);

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        // If there are no more items left that match the drop zones, the game is complete
        if (!items.Any(i => dropZones.Exists(d => d.target == i.item))) OnGameComplete?.Invoke();
    }

    public void AddDropZone(DropZone newDropZone) => dropZones.Add(newDropZone);

    public void RestartGame()
    {
        // Clear existing items and drop zones
        foreach (var item in items) Destroy(item.gameObject);
        items.Clear();

        foreach (var dropZone in dropZones) Destroy(dropZone.gameObject);
        dropZones.Clear();

        // Restart the game
        Start().Forget();
    }
}