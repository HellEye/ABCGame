using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemRenderer : MonoBehaviour, IElementRenderer {
    [SerializeField] SpriteRenderer spriteRenderer;

    AsyncOperationHandle<Sprite> handle;

    void OnDestroy() => AssetReferenceExtensions.Release(handle);

    public async UniTask Initialize(IElement element) {
        if (element is not ItemSO itemData) {
            Debug.LogError(
                $"{nameof(ItemRenderer)} expected {nameof(ItemSO)} but got {element?.GetType().Name ?? "null"}",
                this);
            return;
        }

        handle = itemData.sprite.Load();
        spriteRenderer.sprite = await handle.Task;
    }
}