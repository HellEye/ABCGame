using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragController : MonoBehaviour {
    [Tooltip("Interaction layer for draggable elements")]
    public LayerMask draggableLayer;

    [Tooltip("Layer for drop zone elements")]
    public LayerMask dropLayer;

    [Tooltip("Radius in which the draggable elements are detected around the touch position")]
    public float dragDetectRadius = 0.1f;

    [Tooltip("Drop zone detection radius offset (added to the collider radius)")]
    public float dropZoneDetectRadius = 0.1f;

    [Tooltip(
        "Whether to check for drop targets when moving, requires some extra performance, so disable if not necessary")]
    [SerializeField]
    bool sendHoveredEvents = true;

    [Tooltip(
        "Whether to preserve the offset between the touch position and the draggable element's position. Without it, the item's center 'snaps' to the cursor")]
    [SerializeField]
    bool preserveOffset = true;

    [Inject] InputSystem_Actions actions;

    Camera cam;
    Draggable currentElement;
    DropZone hoveredDropZone;
    bool isDragging;
    Vector2 offset;
    void Start() => cam = Camera.main;

    void OnEnable() {
        actions.Player.Touch.Enable();
        actions.Player.Touch.started += OnTouchStarted;
        actions.Player.Touch.performed += OnTouchMoved;
        actions.Player.Touch.canceled += OnTouchEnded;
    }

    void OnDisable() {
        actions.Player.Touch.Disable();
        actions.Player.Touch.started -= OnTouchStarted;
        actions.Player.Touch.performed -= OnTouchMoved;
        actions.Player.Touch.canceled -= OnTouchEnded;
    }

    Collider2D GetClosestCollider(Vector2 position, LayerMask layerMask, float radius) {
        var hits = Physics2D.OverlapCircleAll(position, radius, layerMask);
        if (hits.Length == 0) return null;

        // Find the closest collider, in case of overlap
        var closest = hits[0];
        foreach (var hit in hits) {
            var closestPosition = new Vector2(closest.transform.position.x, closest.transform.position.y);
            var hitPosition = new Vector2(hit.transform.position.x, hit.transform.position.y);
            if (Vector2.SqrMagnitude(hitPosition - position) < Vector2.SqrMagnitude(closestPosition - position))
                closest = hit;
        }

        return closest;
    }

    void OnTouchStarted(InputAction.CallbackContext callbackContext) {
        var touchPosition = callbackContext.ReadValue<Vector2>().ScreenToWorldPoint2D(cam);
        var closest = GetClosestCollider(touchPosition, draggableLayer, dragDetectRadius);
        if (closest == null) return;
        // Set the current element to the closest one
        if (!closest.TryGetComponent<Draggable>(out var draggable) || !draggable.enabled) return;
        currentElement = draggable;
        draggable.Pickup();
        // offset for smoother dragging (not jumping the center of the element to the mouse)
        offset = preserveOffset ? (Vector2)draggable.transform.position - touchPosition : Vector2.zero;
        isDragging = true;
    }

    void OnTouchMoved(InputAction.CallbackContext callbackContext) {
        if (!isDragging) return;
        var worldPos = callbackContext.ReadValue<Vector2>().ScreenToWorldPoint2D(cam);
        // update the position of the draggable element
        currentElement.Move(worldPos + offset);
        if (sendHoveredEvents) CheckHovered(worldPos);
    }

    void CheckHovered(Vector2 worldPos) {
        if (currentElement == null) return;
        var radius = currentElement.collider.bounds.size.x / 2 + dropZoneDetectRadius;
        var hitCollider = GetClosestCollider(worldPos, dropLayer, radius);
#if UNITY_EDITOR
        var color = hitCollider == null ? Color.orangeRed : Color.darkGreen;
        DebugDraw.Circle(worldPos, radius, color);
#endif
        var dropZone = hitCollider == null ? null : hitCollider.GetComponent<DropZone>();

        // feels kinda ugly, sorry
        if (hoveredDropZone == dropZone) return;
        if (hoveredDropZone != null) hoveredDropZone.IsHovered(false);
        hoveredDropZone = dropZone;
        if (hoveredDropZone != null)
            hoveredDropZone.IsHovered(true);
    }

    void OnTouchEnded(InputAction.CallbackContext callbackContext) {
        // if we were sending hovered events, we need to reset the hovered drop zone
        if (sendHoveredEvents && hoveredDropZone != null) {
            hoveredDropZone.IsHovered(false);
            hoveredDropZone = null;
        }

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
        var name = dropZoneCollider == null ? "null" : dropZoneCollider.name;
        Debug.Log($"Found drop zone: {name}", dropZoneCollider);
        // found, trigger the drop zone
        if (dropZoneCollider == null || !dropZoneCollider.TryGetComponent<DropZone>(out var dropZone)) {
            droppedElement.Drop();


            return;
        }

        dropZone.Drop(droppedElement);
    }
}