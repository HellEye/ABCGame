using System;
using Reflex.Attributes;
using UnityEngine;

[DefaultExecutionOrder(-900)]
[ExecuteAlways]
public class ScreenSizeManager : MonoBehaviour
{
    [Inject] Camera cam;

    public Vector2 SizePx { get; private set; }

    void Start() => cam = Camera.main;

    void Update()
    {
        if (!RecalculateNewPosition(out var newSize)) return;
        OnResize?.Invoke(newSize);
        OnResizeUnits?.Invoke(this);
    }


    void OnValidate()
    {
        if (cam == null) cam = Camera.main;
        RecalculateNewPosition(out var _);
    }

    public bool RecalculateNewPosition(out Vector2 newSize)
    {
        var width = Screen.width;
        var height = Screen.height;
        newSize = new(width, height);
        if (SizePx == newSize) return false;
        SizePx = newSize;
        return true;
    }

    public event Action<Vector2> OnResize;
    public event Action<ScreenSizeManager> OnResizeUnits;


    public Vector3 FromNormalizedToWorldPos(Vector2 normalizedPos, float z = 0) =>
        cam.ViewportToWorldPoint(new Vector3(normalizedPos.x, normalizedPos.y, z)) - cam.transform.position;


    public Vector2 FromWorldToNormalizedPos(Vector3 worldPos) => cam.WorldToViewportPoint(worldPos);

    public float FromWorldToNormalizedDistance(float worldDistance)
    {
        var worldTopRight = cam.ViewportToWorldPoint(Vector3.one) - cam.transform.position;
        var worldBottomLeft = cam.ViewportToWorldPoint(Vector3.zero) - cam.transform.position;
        var worldSize = worldTopRight - worldBottomLeft;
        var normalizedDistance = worldDistance / worldSize.magnitude;
        return normalizedDistance;
    }
}