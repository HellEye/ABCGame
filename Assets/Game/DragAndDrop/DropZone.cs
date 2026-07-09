using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour {
    public VisualEffect correctEffect;
    [SerializeField] ScreenPositionPlacer placer;
    [SerializeField] InterfaceReference<IElementRenderer> itemRenderer;
    [Inject] DropZoneGameManager gameManager;

    AsyncOperationHandle<Sprite> handle;
    public IElement target;

    public void Initialize(IElement item, Vector2 pos) {
        target = item;

        if (itemRenderer.Value == null) {
            Debug.LogError($"{nameof(DropZone)} requires a component implementing {nameof(IElementRenderer)}", this);
            return;
        }

        if (placer != null)
            placer.NormalizedPosition = pos;

        itemRenderer.Value.Initialize(item).Forget();
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