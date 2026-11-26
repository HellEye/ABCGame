using UnityEngine;

[ExecuteInEditMode]
public class BezierWire : MonoBehaviour {
    public LineRenderer line;
    public int segmentCount = 20;
    public Transform lastPoint;
    [SerializeField] Draggable draggable;
    [SerializeField] WireDragAnimations animations;
    [SerializeField] Transform wireEndVisuals;
    void Update() => UpdateLine();

    void OnEnable() {
        if (draggable == null) return;
        SetLineActive(false);
        draggable.OnPickedUp += () => SetLineActive(true);
        animations.OnIncorrectMotionComplete += () => SetLineActive(false);
    }

    void SetLineActive(bool active) {
        line.gameObject.SetActive(active);
        wireEndVisuals.gameObject.SetActive(active);
    }

    void UpdateLine() {
        Vector2 p0 = line.transform.position;
        Vector2 p3 = lastPoint.position;
        var p1 = new Vector2(Mathf.Lerp(p0.x, lastPoint.position.x, 0.3f), p0.y);
        var p2 = new Vector2(Mathf.Lerp(p0.x, lastPoint.position.x, 0.7f), lastPoint.position.y);
        line.positionCount = segmentCount + 1;
        for (var i = 0; i <= segmentCount; i++) {
            var t = i / (float)segmentCount;
            var point = CubicBezier(p0, p1, p2, p3, t);
            line.SetPosition(i, point);
        }
    }

    static Vector2 CubicBezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d,
        float t) {
        var mt = 1 - t;
        return mt * mt * mt * a +
               3 * mt * mt * t * b +
               3 * mt * t * t * c +
               t * t * t * d;
    }
}