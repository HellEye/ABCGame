using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour, ISelectEvents {
    public VisualEffect correctEffect;

    [SerializeField] ScreenPositionPlacer placer;

    [Tooltip(
        "Optional transform to move the draggable to when dropped correctly. If null, the draggable will be dropped at the center of this drop zone.")]
    [SerializeField]
    Transform targetPosition;

    AsyncOperationHandle<Sprite> handle;

    public List<IElement> targets;
    public event Action OnSelected;
    public event Action OnDeselected;

    public void Initialize(IEnumerable<IElement> targets, Vector2 pos) {
        this.targets = targets.ToList();

        if (placer != null)
            placer.NormalizedPosition = pos;
    }

    public void Drop(Draggable draggable) {
        if (targets.Contains(draggable.item.data))
            Correct(draggable);
        else
            Incorrect(draggable);
    }

    public void IsHovered(bool hovered) {
        if (hovered)
            OnSelected?.Invoke();
        else
            OnDeselected?.Invoke();
    }

    void Correct(Draggable draggable) {
        if (correctEffect != null)
            correctEffect.Play();
        draggable.DropCorrect(targetPosition == null ? transform.position : targetPosition.position);
    }

    void Incorrect(Draggable draggable) => draggable.DropIncorrect();
}