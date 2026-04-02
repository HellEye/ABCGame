using UnityEngine;
using System.Collections.Generic;

public class ItemSpawnerManager : MonoBehaviour
{
    [SerializeField] public List<ItemSO> items = new List<ItemSO>(); //to be moved to some gameManager
    public static ItemSpawnerManager Instance { get; private set; }

    public int maxItems;
    public int itemsPerType;
    public int itemTypesSpawned;

    public Item itemPrefab;

    void Start()
    {
        //TrySpawningItemsPerType(maxItems);
        TrySpawningMaxItems(itemsPerType);
        //test one of these two
    }
    private void Awake() 
    {    
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void TrySpawningItemsPerType(int maxItems) {
        int itemsPerType;
        int remaningItemsToSpawn;
        itemsPerType = maxItems / itemTypesSpawned;
        remaningItemsToSpawn = maxItems % itemTypesSpawned;
        
        for (int i = 0; i < itemTypesSpawned-1; i++) {
            for (int j = 0; j < itemsPerType; j++)
            {
                var newItem = Instantiate(itemPrefab, Vector3.zero , Quaternion.identity);
                newItem.Initialize(items[i], RandomiseSpawnPos());
            }
        }
        for (int i = 0; i < itemsPerType + remaningItemsToSpawn; i++)
        {
            var newItem = Instantiate(itemPrefab, Vector3.zero , Quaternion.identity);
            newItem.Initialize(items[items.Count-1], RandomiseSpawnPos());
        }
        
        //add items to list in a game manager
    }

    public void TrySpawningMaxItems(int itemsPerType) {
        for (int i = 0; i < itemTypesSpawned; i++) {
            for (int j = 0; j < itemsPerType; j++)
            {
                var newItem = Instantiate(itemPrefab, Vector3.zero , Quaternion.identity);
                newItem.Initialize(items[i], RandomiseSpawnPos());
            }
        }
        
        //add items to list in a game manager
    }

    public Vector3 RandomiseSpawnPos() {
        float xPos = UnityEngine.Random.Range(0.0f, 1.0f);
        float yPos = UnityEngine.Random.Range(0.0f, 1.0f);
        
        return new Vector3(xPos, yPos, 0);
    }
}
