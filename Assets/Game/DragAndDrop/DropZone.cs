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
        var item = draggable.GetComponent<Item>();
        if (targets.Contains(item.data))
            Correct(draggable, item);
        else
            Incorrect(draggable);
    }

    void Correct(Draggable draggable, Item item) {
        correctEffect.Play();
        draggable.DropCorrect();
        gameManager.RemoveItem(item);
    }

    void Incorrect(Draggable draggable) => draggable.DropIncorrect();
}