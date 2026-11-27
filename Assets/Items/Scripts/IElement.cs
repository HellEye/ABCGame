using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IElement {
    string DisplayNamePlural { get; }
    string DisplayNameSingular { get; }
    bool IsPlaceholder { get; }
    AssetReferenceT<Sprite> Icon { get; }
}