using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour {
    public VisualEffect correctEffect;

    [SerializeField] ScreenPositionPlacer placer;

    // [SerializeField] InterfaceReference<IElementRenderer> itemRenderer;
    [Inject] DropZoneGameManager gameManager;

    AsyncOperationHandle<Sprite> handle;
    public List<IElement> targets;

    public void Initialize(IEnumerable<IElement> targets, Vector2 pos) {
        this.targets = targets.ToList();

        // if (itemRenderer.Value == null) {
        //     Debug.LogError($"{nameof(DropZone)} requires a component implementing {nameof(IElementRenderer)}", this);
        //     return;
        // }

        if (placer != null)
            placer.NormalizedPosition = pos;

        // itemRenderer.Value.Initialize(item).Forget();
    }

    public void Drop(Draggable draggable) {
        if (targets.Contains(draggable.item.data))
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