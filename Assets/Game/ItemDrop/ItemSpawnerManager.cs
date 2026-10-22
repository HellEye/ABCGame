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

    public DropZone SpawnDropZone(IEnumerable<IElement> targets) {
        var newDropZone = Instantiate(dropZonePrefab, Vector3.zero, Quaternion.identity);
        newDropZone.Initialize(targets, new(0.5f, dropZoneNormalizedY));
        return newDropZone;
    }
}