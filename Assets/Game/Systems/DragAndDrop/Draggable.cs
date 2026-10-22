using System;
using UnityEngine;

public enum DropType {
    Empty,
    Correct,
    Incorrect
}

public struct DropInfo {
    public DropType dropType;
    public Vector3 targetPos;
    public Vector3 startPos;
    public Item item;

    public override string ToString() =>
        $"DropInfo(dropType={dropType}, targetPos={targetPos}, startPos={startPos}, item={item})";
}

[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour {
    public new Collider2D collider;
    public Item item;
    Vector3 startPos;

    void Awake() => collider = GetComponent<Collider2D>();


    public event Action<DropInfo> OnDropped;
    public event Action OnPickedUp;
    public void Move(Vector2 newPosition) => transform.position = new(newPosition.x, newPosition.y);

    public void Pickup() {
        startPos = transform.position;
        OnPickedUp?.Invoke();
    }

    DropInfo CreateDropInfo(DropType dropType, Vector3? targetPos = null) =>
        new() {
            dropType = dropType,
            targetPos = targetPos ?? transform.position,
            startPos = startPos,
            item = item
        };

    public void Drop() => OnDropped?.Invoke(CreateDropInfo(DropType.Empty));

    public void DropCorrect(Vector3? targetPos = null) =>
        OnDropped?.Invoke(CreateDropInfo(DropType.Correct, targetPos));

    public void DropIncorrect() => OnDropped?.Invoke(CreateDropInfo(DropType.Incorrect));
}