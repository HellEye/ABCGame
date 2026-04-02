using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnerManager : MonoBehaviour
{
    [SerializeField]
    public List<ItemSO> items = new(); //to be moved to some gameManager

    public int maxItems;
    public int itemsPerType;
    public int itemTypesSpawned;

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

    void Start() =>
        //TrySpawningItemsPerType(maxItems);
        TrySpawningMaxItems(itemsPerType);

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
    public void TrySpawningItemsPerType(int maxItems)
    {
        var itemsPerType = maxItems / itemTypesSpawned;
        var remaningItemsToSpawn = maxItems % itemTypesSpawned;

        for (var i = 0; i < itemTypesSpawned - 1; i++)
        for (var j = 0; j < itemsPerType; j++)
        {
            var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            newItem.Initialize(items[i], RandomiseSpawnPos());
        }

        for (var i = 0; i < itemsPerType + remaningItemsToSpawn; i++)
        {
            var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            newItem.Initialize(items[^1], RandomiseSpawnPos());
        }

        //add items to list in a game manager
    }

    public void TrySpawningMaxItems(int itemsPerType)
    {
        for (var i = 0; i < itemTypesSpawned; i++)
        for (var j = 0; j < itemsPerType; j++)
        {
            var newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            newItem.Initialize(items[i], RandomiseSpawnPos());
        }

        //add items to list in a game manager
    }

    public Vector3 RandomiseSpawnPos()
    {
        var xPos = Random.Range(bounds.min.x, bounds.max.x);
        var yPos = Random.Range(bounds.min.y, bounds.max.y);

        return new(xPos, yPos, 0);
    }
}