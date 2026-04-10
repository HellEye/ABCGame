using System;
using UnityEngine;

[Serializable]
public struct MinMaxRect {
    public Vector2 min;
    public Vector2 max;

    public MinMaxRect(Vector2 min, Vector2 max) {
        this.min = min;
        this.max = max;
    }

    public Vector2 Size => max - min;

    public Rect ToRect() => new(min, Size);
}