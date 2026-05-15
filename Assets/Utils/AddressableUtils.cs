using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetReferenceExtensions {
    public static AsyncOperationHandle<T> Load<T>(
        this AssetReferenceT<T> reference
    ) where T : Object =>
        // Addressables.LoadAssetAsync handles ref counting
        // Calling it again just increments the count
        Addressables.LoadAssetAsync<T>(reference);

    public static void Release<T>(this AsyncOperationHandle<T> handle) where T : Object => Addressables.Release(handle);
}