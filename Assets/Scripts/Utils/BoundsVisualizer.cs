using UnityEngine;

internal interface IScreenSpaceBounds {
    MinMaxRect Bounds { get; }
}

[ExecuteAlways]
public class BoundsVisualizer : MonoBehaviour {
    [SerializeField] ScreenSizeManager screenSizeManager;

    [SerializeField] InterfaceReference<IScreenSpaceBounds> screenBounds;

    /// <summary>
    ///     Draws the spawning area
    /// </summary>
    void OnDrawGizmosSelected() {
        if (screenSizeManager == null) {
            Debug.LogError("Screen size manager not found on ItemSpawnerManager, bounds not drawn");
            return;
        }

        if (screenBounds?.Value == null) return;
        var bounds = screenBounds.Value.Bounds;

        screenSizeManager.RecalculateNewPosition(out var _);

        Gizmos.color = Color.blue;
        var pos = transform.position;
        var bottomLeft = screenSizeManager.FromNormalizedToWorldPos(bounds.min);
        var topRight = screenSizeManager.FromNormalizedToWorldPos(bounds.max);
        var topLeft = new Vector3(bottomLeft.x, topRight.y, 0);
        var bottomRight = new Vector3(topRight.x, bottomLeft.y, 0);
        Gizmos.DrawLine(pos + bottomLeft, pos + topLeft);
        Gizmos.DrawLine(pos + topRight, pos + bottomRight);
        Gizmos.DrawLine(pos + bottomLeft, pos + bottomRight);
        Gizmos.DrawLine(pos + topLeft, pos + topRight);
    }
}