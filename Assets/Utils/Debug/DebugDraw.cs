using UnityEngine;

public static class DebugDraw {
    public static void Circle(
        Vector2 center,
        float radius,
        Color color,
        float duration = 0f,
        int segments = 32) {
        var prev = center + new Vector2(radius, 0f);
        for (var i = 1; i <= segments; i++) {
            var angle = i * Mathf.PI * 2f / segments;
            var next =
                center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prev, next, color, duration);
            prev = next;
        }
    }
}