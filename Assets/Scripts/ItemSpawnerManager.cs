using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnerManager : MonoBehaviour
{
    public int maxItems;
    public int itemsPerType;
    public int itemTypesSpawned;

    [SerializeField]
    float spawnDelay = 0.2f;

    [SerializeField]
    DropZoneGameManager gameManager;

    [SerializeField]
    MinMaxRect bounds;

    [SerializeField]
    ScreenSizeManager screenSizeManager;

    public Item itemPrefab;
    public static ItemSpawnerManager Instance { get; private set; }

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance == null || Instance == this)
            Instance = this;
    }

    /// <summary>
    ///     Draws the spawning area
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (screenSizeManager == null)
        {
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
    public async UniTaskVoid TrySpawningItemsPerType(List<ItemSO> items)
    {
        var itemsPerType = maxItems / itemTypesSpawned;
        var remaningItemsToSpawn = maxItems % itemTypesSpawned;

        for (var i = 0; i < itemTypesSpawned - 1; i++)
            for (var j = 0; j < itemsPerType; j++)
                await CreateItem(items[i]);

        for (var i = 0; i < itemsPerType + remaningItemsToSpawn; i++) await CreateItem(items[^1]);
    }

    public async UniTaskVoid TrySpawningMaxItems(List<ItemSO> items)
    {
        for (var i = 0; i < itemTypesSpawned; i++)
            for (var j = 0; j < itemsPerType; j++)
                await CreateItem(items[i]);
    }

    UniTask CreateItem(ItemSO item)
    {
        var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        newItem.Initialize(item, RandomiseSpawnPos());
        gameManager.AddItem(newItem);
        return UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
    }

    public Vector3 RandomiseSpawnPos()
    {
        var xPos = Random.Range(bounds.min.x, bounds.max.x);
        var yPos = Random.Range(bounds.min.y, bounds.max.y);

        return new(xPos, yPos, 0);
    }
}