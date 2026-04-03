using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropZoneGameManager : MonoBehaviour
{
    [SerializeField]
    List<ItemSO> allItems;

    [SerializeField]
    List<DropZone> dropZones;

    [SerializeField]
    ItemSpawnerManager itemSpawnerManager;

    [SerializeField]
    int itemTypes = 3;

    readonly List<Item> items = new();

    void Start()
    {
        var pickedItems = allItems.PickRandom(itemTypes);
        //itemSpawnerManager.TrySpawningItemsPerType(pickedItems);
        dropZones.ForEach(d => d.SetManager(this));
        itemSpawnerManager.TrySpawningMaxItems(pickedItems).Forget();
        OnGameComplete += () => Debug.Log("Game Complete!!!");
    }


    public event Action OnGameComplete;
    public void AddItem(Item item) => items.Add(item);

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        var remainingValidItems = items.Count(i => dropZones.Select(d => d.target).Contains(i.item));
        if (remainingValidItems == 0) OnGameComplete?.Invoke();
    }
}