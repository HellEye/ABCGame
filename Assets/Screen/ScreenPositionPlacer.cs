using Reflex.Attributes;
using UnityEngine;

public class ScreenPositionPlacer : MonoBehaviour {
    [SerializeField] Vector2 pos;

    [Inject] [SerializeField] [HideInInspector]
    ScreenSizeManager screenSizeManager;

    [SerializeField] float zIndex;

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

    void OnEnable() => screenSizeManager.OnResizeUnits += OnScreenResize;

    void OnDisable() {
        if (screenSizeManager != null) screenSizeManager.OnResizeUnits -= OnScreenResize;
    }

    void OnScreenResize(ScreenSizeManager screenManager) =>
        transform.position = screenManager.FromNormalizedToWorldPos(NormalizedPosition, zIndex);
}