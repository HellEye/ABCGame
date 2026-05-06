using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemSO : ScriptableObject, ITarget {
    public string itemName;
    [SerializeField] Sprite sprite2D;
    public Sprite Sprite2D => sprite2D;
    public bool Matches(Draggable draggable) => draggable.item.item == this;
}