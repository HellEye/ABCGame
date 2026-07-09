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
    public void TrySpawningItemsPerType(IEnumerable<IElement> items) {
        var itemList = items.ToList();
        var itemsPerType = difficulty.maxItems / itemList.Count;
        var remainingItemsToSpawn = difficulty.maxItems % itemList.Count;

        var itemPositions = new List<Vector2>(itemList.Count * difficulty.itemsPerType);
        var squareRadius =
            screenSizeManager.FromWorldToNormalizedDistance(spawnProtectionRadius * spawnProtectionRadius);

        for (var i = 0; i < itemList.Count - 1; i++)
        for (var j = 0; j < itemsPerType; j++)
            itemPositions.Add(
                CreateItem(
                    itemList[i],
                    RandomiseSpawnPos(itemPositions, squareRadius)
                ).screenPlacer.NormalizedPosition
            );

        for (var i = 0; i < itemsPerType + remainingItemsToSpawn; i++)
            itemPositions.Add(
                CreateItem(
                    itemList[^1],
                    RandomiseSpawnPos(itemPositions, squareRadius)
                ).screenPlacer.NormalizedPosition
            );
    }

    public void TrySpawningMaxItems(IEnumerable<IElement> items) {
        var itemPositions = new List<Vector2>(items.Count() * difficulty.itemsPerType);
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

    Item CreateItem(IElement item, Vector3 pos) {
        var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        newItem.Initialize(item, pos);
        gameManager.AddItem(newItem);
        return newItem;
    }

    void CreateDropZone(IElement item, float normalizedXPos) {
        var newDropZone = Instantiate(dropZonePrefab, Vector3.zero, Quaternion.identity);
        newDropZone.Initialize(item, new(normalizedXPos, dropZoneNormalizedY));
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

    public void SpawnDropZones(IEnumerable<IElement> targets) {
        var targetList = targets.ToList();
        var count = targetList.Count;
        var spacing = 1f / (count + 1f);
        for (var i = 0; i < targetList.Count; i++) {
            var target = targetList[i];
            CreateDropZone(target, spacing * (i + 1));
        }
    }
}