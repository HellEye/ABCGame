using Reflex.Attributes;
using UnityEngine;

public class ScreenPositionPlacer : MonoBehaviour {
    [SerializeField] Vector2 pos;

    [Inject] [SerializeField] [HideInInspector]
    ScreenSizeManager screenSizeManager;

    [SerializeField] float zIndex;
    Vector2 lastNormalizedPos;

    Vector3 lastTransformPos;

    public Vector2 NormalizedPosition { get => pos; set => pos = value; }

    void Update() => UpdatePosition();

    void OnEnable() {
        if (screenSizeManager == null) screenSizeManager = FindFirstObjectByType<ScreenSizeManager>();
        Debug.Assert(screenSizeManager != null, $"No {nameof(ScreenSizeManager)} found in scene", this);
        screenSizeManager.OnResizeUnits += OnScreenResize;
        UpdatePosition();
    }

    void OnDisable() {
        if (screenSizeManager != null) screenSizeManager.OnResizeUnits -= OnScreenResize;
    }


    void UpdatePosition() {
        if (transform.position != lastTransformPos) {
            NormalizedPosition = screenSizeManager.FromWorldToNormalizedPos(transform.position);
            lastNormalizedPos = NormalizedPosition;
            lastTransformPos = transform.position;
        }
        else if (lastNormalizedPos != NormalizedPosition) {
            transform.position = screenSizeManager.FromNormalizedToWorldPos(NormalizedPosition, zIndex);
            lastNormalizedPos = NormalizedPosition;
            lastTransformPos = transform.position;
        }
    }

    void OnScreenResize(ScreenSizeManager screenManager) =>
        transform.position = screenManager.FromNormalizedToWorldPos(NormalizedPosition, zIndex);
}