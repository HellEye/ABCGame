using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(ScreenPositionPlacer))]
public class Item : MonoBehaviour {
    public ItemSO data;

    [SerializeField] [HideInInspector] SpriteRenderer spriteRenderer;

    [SerializeField] ScreenPositionPlacer screenPlacer;

    void OnValidate() {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data != null) spriteRenderer.sprite = data.Sprite2D;
    }

    public void Initialize(ItemSO itemData, Vector3 pos) {
        data = itemData;
        if (screenPlacer == null)
            screenPlacer = GetComponent<ScreenPositionPlacer>();
        screenPlacer.Pos = pos;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.Sprite2D;
    }
}