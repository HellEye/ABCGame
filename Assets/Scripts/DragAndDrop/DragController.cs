using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragController : MonoBehaviour
{
    [Tooltip("Interaction layer for draggable elements")]
    public LayerMask draggableLayer;

    [Tooltip("Layer for drop zone elements")]
    public LayerMask dropLayer;

    [Tooltip("Radius in which the draggable elements are detected around the touch position")]
    public float dragDetectRadius = 0.1f;

    [Tooltip("Drop zone detection radius offset (added to the collider radius)")]
    public float dropZoneDetectRadius = 0.1f;

    Camera cam;
    Input input;
    Draggable currentElement = null;
    bool isDragging = false;
    Vector2 offset;
    void Start() { cam = Camera.main; }

    void OnEnable()
    {
        input = Input.instance;
        input.actions.Player.Touch.started += OnTouchStarted;
        input.actions.Player.Touch.performed += OnTouchMoved;
        input.actions.Player.Touch.canceled += OnTouchEnded;
    }

    void OnDisable()
    {
        input.actions.Player.Touch.started -= OnTouchStarted;
        input.actions.Player.Touch.performed -= OnTouchMoved;
        input.actions.Player.Touch.canceled -= OnTouchEnded;
    }

    Collider2D GetClosestCollider(Vector2 position, LayerMask layerMask, float radius)
    {
        var hits = Physics2D.OverlapCircleAll(position, radius, layerMask);
        if (hits.Length == 0) return null;

        // Find the closest collider, in case of overlap
        var closest = hits[0];
        foreach (var hit in hits)
        {
            var closestPosition = new Vector2(closest.transform.position.x, closest.transform.position.y);
            var hitPosition = new Vector2(hit.transform.position.x, hit.transform.position.y);
            if (Vector2.SqrMagnitude(hitPosition - position) < Vector2.SqrMagnitude(closestPosition - position))
            {
                closest = hit;
            }
        }

        return closest;
    }

    void OnTouchStarted(InputAction.CallbackContext callbackContext)
    {
        var touchPosition = callbackContext.ReadValue<Vector2>().ScreenToWorldPoint2D(cam);
        var closest = GetClosestCollider(touchPosition, draggableLayer, dragDetectRadius);
        if (closest == null) return;
        Debug.Log("Closest hit: " + closest.name);
        // Set the current element to the closest one
        if (!closest.TryGetComponent<Draggable>(out var draggable)) return;
        currentElement = draggable;
        // offset for smoother dragging (not jumping the center of the element to the mouse)
        offset = ((Vector2)draggable.transform.position) - touchPosition;
        isDragging = true;
    }

    void OnTouchMoved(InputAction.CallbackContext callbackContext)
    {
        if (!isDragging) return;
        var worldPos = callbackContext.ReadValue<Vector2>().ScreenToWorldPoint2D(cam);
        // update the position of the draggable element
        currentElement.Move(worldPos + offset);
    }

    void OnTouchEnded(InputAction.CallbackContext callbackContext)
    {
        isDragging = false;
        if (currentElement == null) return;
        var droppedElement = currentElement;
        currentElement = null;

        // Try finding a drop zone
        var position = droppedElement.transform.position;
        // add the collider radius to the drop zone detection radius
        // using bounds instead of radius to support non-circle colliders
        var dropDetectRadius = dropZoneDetectRadius + droppedElement.collider.bounds.size.x / 2;
        var dropZoneCollider = GetClosestCollider(position, dropLayer, dropDetectRadius);

        // not found, do nothing
        if (dropZoneCollider == null) return;

        // found, trigger the drop zone
        if (!dropZoneCollider.TryGetComponent<DropZone>(out var dropZone)) return;
        dropZone.Drop(droppedElement);
    }
}