using UnityEngine;

public static class VectorUtils
{
    /// <summary>
    /// Converts a screen position to a world position in 2D space.
    /// </summary>
    /// <param name="screenPos">screen position (e.g., from input context)</param>
    /// <param name="cam">camera currently rendering the scene</param>
    /// <returns></returns>
    public static Vector2 ScreenToWorldPoint2D(this Vector2 screenPos, Camera cam)
    {
        var distanceFromCamera = -cam.transform.position.z;
        var world = cam.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
        return new Vector2(world.x, world.y);
    }
}