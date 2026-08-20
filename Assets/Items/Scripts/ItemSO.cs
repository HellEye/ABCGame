using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class ItemSO : ScriptableObject, IElement {
    public string itemName;
    public string pluralName;
    public AssetReferenceT<Sprite> sprite;


    AsyncOperationHandle<Sprite> spriteHandle;
    public string TargetDisplayName => pluralName;
    public bool Matches(Item item) => ReferenceEquals(item.data, this);

    public override string ToString() => $"ItemSO: {itemName} ({name})";
}