using UnityEngine;

public static class VectorUtils {
    /// <summary>
    ///     Converts a screen position to a world position in 2D space.
    /// </summary>
    /// <param name="screenPos">screen position (e.g., from input context)</param>
    /// <param name="cam">camera currently rendering the scene</param>
    /// <returns></returns>
    public static Vector2 ScreenToWorldPoint2D(this Vector2 screenPos, Camera cam) {
        var distanceFromCamera = -cam.transform.position.z;
        var world = cam.ScreenToWorldPoint(
            new(screenPos.x, screenPos.y, distanceFromCamera));
        return new(world.x, world.y);
    }

    public static Color With(this Color color, float? r = null, float? g = null, float? b = null, float? a = null) =>
        new(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
}