using UnityEngine;

namespace DragAndDrop {
    public interface ITarget {
        public Sprite sprite2D { get; }
        public bool Matches(Draggable draggable);
    }
}