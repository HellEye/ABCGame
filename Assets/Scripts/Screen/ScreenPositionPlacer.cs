using Reflex.Attributes;
using UnityEngine;

[ExecuteAlways]
public class ScreenPositionPlacer : MonoBehaviour {
    [SerializeField] Vector2 pos;

    [Inject] [SerializeField] [HideInInspector]
    ScreenSizeManager screenSizeManager;

    Vector2 lastNormalizedPos;

    Vector3 lastTransformPos;

    public Vector2 NormalizedPosition {
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

    void OnEnable() => screenSizeManager.OnResizeUnits += OnScreenResize;

    void OnDisable() {
        if (screenSizeManager != null) screenSizeManager.OnResizeUnits -= OnScreenResize;
    }

    void OnValidate() {
        if (screenSizeManager == null) screenSizeManager = FindFirstObjectByType<ScreenSizeManager>();
        if (screenSizeManager == null) return;
        if (transform.position != lastTransformPos) {
            NormalizedPosition = screenSizeManager.FromWorldToNormalizedPos(transform.position);
            lastNormalizedPos = NormalizedPosition;
            lastTransformPos = transform.position;
        }
        else if (lastNormalizedPos != NormalizedPosition) {
            transform.position = screenSizeManager.FromNormalizedToWorldPos(NormalizedPosition);
            lastNormalizedPos = NormalizedPosition;
            lastTransformPos = transform.position;
        }
    }

    void OnScreenResize(ScreenSizeManager screenManager) =>
        transform.position = screenManager.FromNormalizedToWorldPos(NormalizedPosition);
}