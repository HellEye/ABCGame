using System;
using UnityEngine;

public class DraggableSelectEvents : MonoBehaviour, ISelectEvents {
    [SerializeField] Draggable draggable;

    void OnEnable() {
        draggable.OnPickedUp += OnDragStart;
        draggable.OnDropped += OnDragEnd;
    }

    void OnDisable() {
        draggable.OnPickedUp -= OnDragStart;
        draggable.OnDropped -= OnDragEnd;
    }

    public event Action OnSelected;
    public event Action OnDeselected;

    void OnDragStart() => OnSelected?.Invoke();

    void OnDragEnd(DropInfo _) => OnDeselected?.Invoke();
}