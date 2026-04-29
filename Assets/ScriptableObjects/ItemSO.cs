using DragAndDrop;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemSO : ScriptableObject, ITarget {
    public string itemName;
    public Sprite sprite2D { get; }
    public bool Matches(Draggable draggable) => draggable.item.item == this;
}