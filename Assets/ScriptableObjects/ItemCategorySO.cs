using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/Category", order = 1)]
public class ItemCategorySO : ScriptableObject {
    public string title;
    public List<ItemSO> items;

    public List<ItemSO> PickRandom(int count) => items.PickRandom(count);
}