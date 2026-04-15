using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnerManager : MonoBehaviour {
    [SerializeField] float spawnDelay = 0.2f;
    [SerializeField] MinMaxRect bounds;
    [SerializeField] float dropZoneNormalizedY = 0.2f;

    [Header("Prefabs")]
    [SerializeField] Item itemPrefab;

    [SerializeField] DropZone dropZonePrefab;

    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] ScreenSizeManager screenSizeManager;

    DropZoneGameDifficulty difficulty;

    /// <summary>
    ///     Draws the spawning area
    /// </summary>
    void OnDrawGizmosSelected() {
        if (screenSizeManager == null) {
            Debug.LogError("Screen size manager not found on ItemSpawnerManager, bounds not drawn");
            return;
        }

        screenSizeManager.RecalculateNewPosition(out var _);

        Gizmos.color = Color.blue;
        var pos = transform.position;
        var bottomLeft = screenSizeManager.FromNormalizedToWorldPos(bounds.min);
        var topRight = screenSizeManager.FromNormalizedToWorldPos(bounds.max);
        var topLeft = new Vector3(bottomLeft.x, topRight.y, 0);
        var bottomRight = new Vector3(topRight.x, bottomLeft.y, 0);
        Gizmos.DrawLine(pos + bottomLeft, pos + topLeft);
        Gizmos.DrawLine(pos + topRight, pos + bottomRight);
        Gizmos.DrawLine(pos + bottomLeft, pos + bottomRight);
        Gizmos.DrawLine(pos + topLeft, pos + topRight);
    }

    //test one of these two
    public async UniTaskVoid TrySpawningItemsPerType(List<ItemSO> items) {
        var itemsPerType = difficulty.maxItems / items.Count;
        var remainingItemsToSpawn = difficulty.maxItems % items.Count;

        for (var i = 0; i < items.Count - 1; i++)
        for (var j = 0; j < itemsPerType; j++)
            await CreateItem(items[i]);

        for (var i = 0; i < itemsPerType + remainingItemsToSpawn; i++) await CreateItem(items[^1]);
    }

    public async UniTask TrySpawningMaxItems(List<ItemSO> items) {
        foreach (var t in items)
            for (var j = 0; j < difficulty.itemsPerType; j++)
                await CreateItem(t);
    }

    UniTask CreateItem(ItemSO item) {
        var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        newItem.Initialize(item, RandomiseSpawnPos());
        gameManager.AddItem(newItem);
        return UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
    }

    UniTask CreateDropZone(ItemSO item, float normalizedXPos) {
        var newDropZone = Instantiate(dropZonePrefab, Vector3.zero, Quaternion.identity);
        newDropZone.SetManager(gameManager);
        newDropZone.Initialize(item, new(normalizedXPos, dropZoneNormalizedY));
        gameManager.AddDropZone(newDropZone);
        return UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
    }

    public Vector3 RandomiseSpawnPos() {
        var xPos = Random.Range(bounds.min.x, bounds.max.x);
        var yPos = Random.Range(bounds.min.y, bounds.max.y);

        return new(xPos, yPos, 0);
    }

    public async UniTask SpawnDropZones(List<ItemSO> targets) {
        var count = targets.Count;
        var spacing = 1f / (count + 1f);
        for (var i = 0; i < targets.Count; i++) {
            var target = targets[i];
            await CreateDropZone(target, spacing * (i + 1));
        }
    }

    public void SetDifficulty(DropZoneGameDifficulty dropZoneGameDifficulty) => difficulty = dropZoneGameDifficulty;
}