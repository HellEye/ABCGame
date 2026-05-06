using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Item))]
public class Draggable : MonoBehaviour {
    public new Collider2D collider;
    public Item item;
    Vector3 startPos;

    void Awake() {
        collider = GetComponent<Collider2D>();
        item = GetComponent<Item>();
    }

    public event Action<Vector3> OnDroppedIncorrect;
    public event Action OnDroppedCorrect;
    public event Action OnDropped;
    public event Action OnPickedUp;
    public void Move(Vector2 newPosition) => transform.position = new(newPosition.x, newPosition.y);

    public void Pickup() {
        startPos = transform.position;
        OnPickedUp?.Invoke();
    }

    public void Drop() => OnDropped?.Invoke();

    public void DropCorrect() {
        OnDroppedCorrect?.Invoke();
        Debug.Log("Correct");
    }

    public void DropIncorrect() {
        OnDroppedIncorrect?.Invoke(startPos);
        Debug.Log("Incorrect");
    }
}