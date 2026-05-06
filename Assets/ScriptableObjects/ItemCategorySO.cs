using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/Category", order = 1)]
public class ItemCategorySO : ScriptableObject, ITarget {
    public string title;
    public List<ItemSO> items;
    [SerializeField] Sprite sprite2D;
    public Sprite Sprite2D => sprite2D;
    public bool Matches(Draggable draggable) => items.Exists(i => i.Matches(draggable));

    public List<ItemSO> PickRandom(int count) => items.PickRandom(count);
}