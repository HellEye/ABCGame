using UnityEngine;

[ExecuteAlways]
public class ScreenPositionPlacer : MonoBehaviour {
    [SerializeField] Vector2 pos;

    [SerializeField] ScreenSizeManager screenSizeManager;

    Vector2 lastNormalizedPos;

    Vector3 lastTransformPos;

    public Vector2 Pos {
        get => pos;
        set {
            pos = value;
            if (screenSizeManager != null)
                OnScreenResize(screenSizeManager);
        }
    }

    void Update() {
        if (lastTransformPos != transform.position) OnValidate();
    }

    void OnEnable() {
        screenSizeManager ??= ScreenSizeManager.Instance;
        screenSizeManager.OnResizeUnits += OnScreenResize;
    }

    void OnDisable() {
        if (screenSizeManager != null) screenSizeManager.OnResizeUnits -= OnScreenResize;
    }

    void OnValidate() {
        if (screenSizeManager == null) screenSizeManager = FindFirstObjectByType<ScreenSizeManager>();
        if (transform.position != lastTransformPos) {
            Pos = screenSizeManager.FromWorldToNormalizedPos(transform.position);
            lastNormalizedPos = Pos;
            lastTransformPos = transform.position;
        }
        else if (lastNormalizedPos != Pos) {
            transform.position = screenSizeManager.FromNormalizedToWorldPos(Pos);
            lastNormalizedPos = Pos;
            lastTransformPos = transform.position;
        }
    }

    void OnScreenResize(ScreenSizeManager screenManager) =>
        transform.position = screenManager.FromNormalizedToWorldPos(Pos);
}