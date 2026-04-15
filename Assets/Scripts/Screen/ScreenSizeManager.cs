using System;
using UnityEngine;

[DefaultExecutionOrder(-900)]
[ExecuteAlways]
public class ScreenSizeManager : MonoBehaviour {
    [SerializeField] Camera cam;

    public static ScreenSizeManager Instance { get; private set; }

    public Vector2 SizePx { get; private set; }
    // public Vector2 WorldTopRight { get; private set; }
    // public Vector2 WorldBottomLeft { get; private set; }

    void Awake() {
        if (Instance == null && Instance != this)
            Instance = this;
    }

    void Start() => cam = Camera.main;

    void Update() {
        if (!RecalculateNewPosition(out var newSize)) return;
        OnResize?.Invoke(newSize);
        OnResizeUnits?.Invoke(this);
    }

    void OnEnable() {
        if (Instance == null) Instance = this;
    }

    void OnValidate() {
        if (cam == null) cam = Camera.main;
        RecalculateNewPosition(out var _);
    }

    public bool RecalculateNewPosition(out Vector2 newSize) {
        var width = Screen.width;
        var height = Screen.height;
        newSize = new(width, height);
        if (SizePx == newSize) return false;
        SizePx = newSize;
        CalcUnitSize(newSize);
        return true;
    }

    public event Action<Vector2> OnResize;
    public event Action<ScreenSizeManager> OnResizeUnits;

    void CalcUnitSize(Vector2 newSize) {
        // WorldBottomLeft = cam.ViewportToWorldPoint(new(0, 0, -cam.transform.position.z));
        // WorldTopRight = cam.ViewportToWorldPoint(new(newSize.x, newSize.y, -cam.transform.position.z));
    }

    public Vector3 FromNormalizedToWorldPos(Vector2 normalizedPos) =>
        cam.ViewportToWorldPoint(normalizedPos) - cam.transform.position;

    // var x = Mathf.Lerp(WorldBottomLeft.x, WorldTopRight.x, normalizedPos.x);
    // var y = Mathf.Lerp(WorldBottomLeft.y, WorldTopRight.y, normalizedPos.y);
    // return new(x, y, 0f);
    public Vector2 FromWorldToNormalizedPos(Vector3 worldPos) => cam.WorldToViewportPoint(worldPos);
    // var x = Mathf.InverseLerp(WorldBottomLeft.x, WorldTopRight.x, worldPos.x);
    // var y = Mathf.InverseLerp(WorldBottomLeft.y, WorldTopRight.y, worldPos.y);
    // return new(x, y);
}