using Reflex.Attributes;
using UnityEngine;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour {
    public VisualEffect correctEffect;
    public ItemSO target;
    [SerializeField] SpriteRenderer targetSpriteRenderer;
    [SerializeField] ScreenPositionPlacer placer;
    [Inject] DropZoneGameManager gameManager;

    void OnValidate() {
        if (targetSpriteRenderer != null && target != null) targetSpriteRenderer.sprite = target.Sprite2D;
    }

    public void Initialize(ItemSO item, Vector2 pos) {
        target = item;
        if (targetSpriteRenderer != null)
            targetSpriteRenderer.sprite = item.Sprite2D;
        if (placer != null)
            placer.Pos = pos;
    }

    public void Drop(Draggable draggable) {
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