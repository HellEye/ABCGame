using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BackgroundController : MonoBehaviour {
    public List<AssetReferenceSprite> backgrounds;
    [SerializeField] SpriteRenderer spriteRenderer;
    float aspectRatio;

    AsyncOperationHandle<Sprite> handle;
    [Inject] DifficultyHolder holder;
    [Inject] Camera mainCamera;
    float screenAspect;
    Sprite sprite;
    [Inject] BackgroundStore store;
    void Start() => SetNextBackground().Forget();

    void Update() {
        // calculate scales to make sure the background sprite extends the full width of the screen
        // Skip null sprite in case it's not yet loaded, or transform for after the object got destroyed
        if (sprite == null || transform == null) return;
        var cameraHeight = 2f * mainCamera.orthographicSize;
        var cameraWidth = cameraHeight * mainCamera.aspect;

        var spriteWidth = sprite.bounds.size.x;
        var spriteHeight = sprite.bounds.size.y;

        var scaleX = cameraWidth / spriteWidth;
        var scaleY = cameraHeight / spriteHeight;

        // Use the larger scale to ensure the sprite covers the entire screen
        var scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new(scale, scale, 1f);
    }

    void OnDestroy() {
        handle.Release();
        sprite = null;
    }

    async UniTask SetNextBackground() {
        // Load the preselected shuffle-random background
        // Should be preloaded by GameLoader, so await is immediate.
        var mapping = holder.selectedMapping;
        var asset = store.GetCurrentBg(mapping);
        handle = asset.Load();
        await handle.ToUniTask();
        sprite = handle.Result;
        spriteRenderer.sprite = sprite;
    }
}