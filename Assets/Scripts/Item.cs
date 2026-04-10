using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(ScreenPositionPlacer))]
public class Item : MonoBehaviour {
    public ItemSO item;

    [SerializeField] [HideInInspector] SpriteRenderer spriteRenderer;

    [SerializeField] ScreenPositionPlacer screenPlacer;

    void OnValidate() {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && item != null) spriteRenderer.sprite = item.sprite2D;
    }

    public void Initialize(ItemSO item, Vector3 pos) {
        this.item = item;
        if (screenPlacer == null)
            screenPlacer = GetComponent<ScreenPositionPlacer>();
        screenPlacer.Pos = pos;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite2D;
    }
}