using UnityEngine;

public class Item : MonoBehaviour {
    public ScreenPositionPlacer screenPlacer;
    [SerializeField] InterfaceReference<IElementRenderer> renderer;
    public IElement data;

    public void Initialize(IElement itemData, Vector3 pos) {
        data = itemData;
        if (screenPlacer == null)
            screenPlacer = GetComponent<ScreenPositionPlacer>();
        screenPlacer.NormalizedPosition = pos;

        if (renderer.Value == null) {
            Debug.LogError($"{nameof(Item)} requires a component implementing {nameof(IElementRenderer)}", this);
            return;
        }

        renderer.Value.Initialize(itemData);
    }
}