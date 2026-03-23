using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    public new Collider2D collider;
    void Awake() { collider = GetComponent<Collider2D>(); }
    public void Move(Vector2 newPosition) { transform.position = new(newPosition.x, newPosition.y); }
}