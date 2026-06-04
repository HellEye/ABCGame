using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class ItemSO : ScriptableObject {
    public string itemName;
    public AssetReferenceT<Sprite> sprite;


    AsyncOperationHandle<Sprite> spriteHandle;
    public bool Matches(Item item) => item.data == this;

    public override string ToString() => $"ItemSO: {itemName} ({name})";
}