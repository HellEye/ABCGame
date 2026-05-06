using UnityEngine;

public interface ITarget {
    public Sprite Sprite2D { get; }
    public bool Matches(Draggable draggable);
}
