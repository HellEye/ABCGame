using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Item : MonoBehaviour {
    public ScreenPositionPlacer screenPlacer;
    [SerializeField] new InterfaceReference<IElementRenderer> renderer;
    [SerializeField] [CanBeNull] Draggable draggable;
    [SerializeField] List<Item> children = new();
    public IElement data;
    [CanBeNull] public Draggable Draggable => draggable;

    public void Initialize(IElement itemData, Vector3 pos, int depth = 0) {
        if (depth > 10) {
            Debug.LogError("Item depth exceeded 10, possible infinite loop in item hierarchy");
            return;
        }

        data = itemData;
        if (screenPlacer == null)
            screenPlacer = GetComponent<ScreenPositionPlacer>();
        screenPlacer.NormalizedPosition = pos;
        if (renderer.UnderlyingValue != null)
            renderer.Value?.Initialize(itemData);
        foreach (var child in children) child.Initialize(itemData, pos, depth + 1);
    }
}