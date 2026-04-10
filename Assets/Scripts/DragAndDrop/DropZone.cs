using UnityEngine;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour {
    public VisualEffect correctEffect;
    public ItemSO target;
    public SpriteRenderer targetSpriteRenderer;
    [SerializeField] ScreenPositionPlacer placer;
    DropZoneGameManager gameManager;

    void OnValidate() {
        if (targetSpriteRenderer != null && target != null) targetSpriteRenderer.sprite = target.sprite2D;
    }

    public void SetManager(DropZoneGameManager manager) => gameManager = manager;

    public void Initialize(ItemSO item, Vector2 pos) {
        target = item;
        if (targetSpriteRenderer != null)
            targetSpriteRenderer.sprite = item.sprite2D;
        if (placer != null)
            placer.Pos = pos;
    }

    public void Drop(Draggable draggable) {
        Debug.Log("Dropped " + draggable.name);
        if (draggable.item.item == target)
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