using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnerManager : MonoBehaviour, IScreenSpaceBounds {
    [Header("Settings")]
    [SerializeField] MinMaxRect bounds;

    [SerializeField] float dropZoneNormalizedY = 0.2f;
    [SerializeField] float spawnProtectionRadius = 0.2f;

    [Header("Prefabs")]
    [SerializeField] Item itemPrefab;

    [SerializeField] DropZone dropZonePrefab;

    [Inject] DropZoneGameDifficulty difficulty;

    [Header("References")]
    [Inject] DropZoneGameManager gameManager;

    [Inject] ScreenSizeManager screenSizeManager;

    public MinMaxRect Bounds => bounds;

    //test one of these two
    public void TrySpawningItemsPerType(List<ItemSO> items) {
        var itemsPerType = difficulty.maxItems / items.Count;
        var remainingItemsToSpawn = difficulty.maxItems % items.Count;

        var itemPositions = new List<Vector2>(items.Count * difficulty.itemsPerType);
        var squareRadius =
            screenSizeManager.FromWorldToNormalizedDistance(spawnProtectionRadius * spawnProtectionRadius);

        for (var i = 0; i < items.Count - 1; i++)
        for (var j = 0; j < itemsPerType; j++)
            itemPositions.Add(
                CreateItem(
                    items[i],
                    RandomiseSpawnPos(itemPositions, squareRadius)
                ).screenPlacer.NormalizedPosition
            );

        for (var i = 0; i < itemsPerType + remainingItemsToSpawn; i++)
            itemPositions.Add(
                CreateItem(
                    items[^1],
                    RandomiseSpawnPos(itemPositions, squareRadius)
                ).screenPlacer.NormalizedPosition
            );
    }

    public void TrySpawningMaxItems(List<ItemSO> items) {
        var itemPositions = new List<Vector2>(items.Count * difficulty.itemsPerType);
        var squareRadius =
            screenSizeManager.FromWorldToNormalizedDistance(spawnProtectionRadius * spawnProtectionRadius);
        foreach (var t in items)
            for (var j = 0; j < difficulty.itemsPerType; j++)
                itemPositions.Add(
                    CreateItem(
                        t,
                        RandomiseSpawnPos(itemPositions, squareRadius)
                    ).screenPlacer.NormalizedPosition
                );
    }

    Item CreateItem(ItemSO item, Vector3 pos) {
        var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        newItem.Initialize(item, pos).Forget();
        gameManager.AddItem(newItem);
        return newItem;
    }

    void CreateDropZone(ItemSO item, float normalizedXPos) {
        var newDropZone = Instantiate(dropZonePrefab, Vector3.zero, Quaternion.identity);
        newDropZone.Initialize(item, new(normalizedXPos, dropZoneNormalizedY)).Forget();
        gameManager.AddDropZone(newDropZone);
    }

    Vector3 RandomiseSpawnPos(List<Vector2> existingPositions, float squareRadius) {
        var pos = Vector2.zero;
        for (var i = 0; i < 100; i++) {
            var xPos = Random.Range(bounds.min.x, bounds.max.x);
            var yPos = Random.Range(bounds.min.y, bounds.max.y);
            pos = new(xPos, yPos);
            if (existingPositions.All(p => Vector3.SqrMagnitude(p - pos) > squareRadius))
                return pos;
        }

        Debug.LogWarning("Could not find a non-overlapping spawn pos");
        return pos;
    }

    public void SpawnDropZones(List<ItemSO> targets) {
        var count = targets.Count;
        var spacing = 1f / (count + 1f);
        for (var i = 0; i < targets.Count; i++) {
            var target = targets[i];
            CreateDropZone(target, spacing * (i + 1));
        }
    }
}