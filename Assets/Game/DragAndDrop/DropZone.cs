using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour {
    public VisualEffect correctEffect;
    public ItemSO target;
    [SerializeField] SpriteRenderer targetSpriteRenderer;
    [SerializeField] ScreenPositionPlacer placer;
    [Inject] DropZoneGameManager gameManager;


    AsyncOperationHandle<Sprite> handle;
    void OnDestroy() => AssetReferenceExtensions.Release(handle);

    public async UniTaskVoid Initialize(ItemSO item, Vector2 pos) {
        target = item;

        if (targetSpriteRenderer != null) {
            handle = item.sprite.Load();
            targetSpriteRenderer.sprite = await handle.Task;
        }

        if (placer != null)
            placer.NormalizedPosition = pos;
    }

    public void Drop(Draggable draggable) {
        if (draggable.item.data == target)
            Correct(draggable);
        else
            Incorrect(draggable);
    }

    void Correct(Draggable draggable) {
        correctEffect.Play();
        draggable.DropCorrect();
        gameManager.RemoveItem(draggable.item);
    }

    void Incorrect(Draggable draggable) => draggable.DropIncorrect();
}