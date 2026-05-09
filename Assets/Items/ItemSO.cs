using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class ItemSO : ScriptableObject {
    public string itemName;
    [SerializeField] Sprite sprite2D;
    public Sprite Sprite2D => sprite2D;
    public bool Matches(Item item) => item.data == this;
}