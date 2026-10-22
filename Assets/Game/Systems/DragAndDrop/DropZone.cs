using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour {
    public VisualEffect correctEffect;

    [SerializeField] ScreenPositionPlacer placer;

    AsyncOperationHandle<Sprite> handle;
    public List<IElement> targets;

    public void Initialize(IEnumerable<IElement> targets, Vector2 pos) {
        this.targets = targets.ToList();

        if (placer != null)
            placer.NormalizedPosition = pos;
    }

    public void Drop(Draggable draggable) {
        Debug.Log(
            $"Dropped {draggable.item.data.DisplayNamePlural}, targets: {string.Join(", ", targets.Select(t => t.DisplayNamePlural))}");
        if (targets.Contains(draggable.item.data))
            Correct(draggable);
        else
            Incorrect(draggable);
    }

    void Correct(Draggable draggable) {
        if (correctEffect != null)
            correctEffect.Play();
        draggable.DropCorrect(transform.position);
    }

    void Incorrect(Draggable draggable) => draggable.DropIncorrect();
}