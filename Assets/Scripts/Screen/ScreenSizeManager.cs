using System;
using UnityEngine;

[DefaultExecutionOrder(-900)]
public class ScreenSizeManager : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    Camera cam;

    public static ScreenSizeManager Instance { get; private set; }

    public Vector2 SizePx { get; private set; }
    public Vector2 SizeUnits { get; private set; }
    public Vector2 WorldBottomLeft { get; private set; }

    void Awake()
    {
        if (Instance == null && Instance != this)
            Instance = this;
    }

    void Start() => cam = Camera.main;

    void Update()
    {
        if (!RecalculateNewPosition(out var newSize)) return;
        OnResize?.Invoke(newSize);
        OnResizeUnits?.Invoke(WorldBottomLeft, SizeUnits);
    }

    void OnEnable()
    {
        if (Instance == null) Instance = this;
    }

    void OnValidate()
    {
        if (cam == null) cam = Camera.main;
        RecalculateNewPosition(out var _);
    }

    bool RecalculateNewPosition(out Vector2 newSize)
    {
        var width = Screen.width;
        var height = Screen.height;
        newSize = new(width, height);
        if (SizePx == newSize) return false;
        SizePx = newSize;
        CalcUnitSize(newSize);
        return true;
    }

    public event Action<Vector2> OnResize;
    public event Action<Vector2, Vector2> OnResizeUnits;

    void CalcUnitSize(Vector2 newSize)
    {
        var worldBottomLeft = cam.ScreenToWorldPoint(new(0, 0, -cam.transform.position.z));
        var worldTopRight = cam.ScreenToWorldPoint(new(newSize.x, newSize.y, -cam.transform.position.z));
        WorldBottomLeft = worldBottomLeft;
        SizeUnits = worldTopRight - worldBottomLeft;
    }

    public Vector3 FromNormalizedToWorldPos(Vector2 normalizedPos)
    {
        var x = Mathf.Lerp(WorldBottomLeft.x, SizeUnits.x + WorldBottomLeft.x, normalizedPos.x);
        var y = Mathf.Lerp(WorldBottomLeft.y, SizeUnits.y + WorldBottomLeft.y, normalizedPos.y);
        return new(x, y, 0f);
    }

    public Vector2 FromWorldToNormalizedPos(Vector3 worldPos)
    {
        var x = Mathf.InverseLerp(WorldBottomLeft.x, SizeUnits.x + WorldBottomLeft.x, worldPos.x);
        var y = Mathf.InverseLerp(WorldBottomLeft.y, SizeUnits.y + WorldBottomLeft.y, worldPos.y);
        return new(x, y);
    }
}