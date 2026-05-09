using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Item : MonoBehaviour {
    public ItemSO data;
    [SerializeField] SpriteRenderer spriteRenderer;

    public ScreenPositionPlacer screenPlacer;

    AsyncOperationHandle<Sprite> handle;
    void OnDestroy() => AssetReferenceExtensions.Release(handle);

    public async UniTaskVoid Initialize(ItemSO itemData, Vector3 pos) {
        data = itemData;
        if (screenPlacer == null)
            screenPlacer = GetComponent<ScreenPositionPlacer>();
        screenPlacer.NormalizedPosition = pos;

        handle = data.sprite.Load();
        spriteRenderer.sprite = await handle.Task;
    }
}