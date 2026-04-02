using UnityEngine;

public class Item : MonoBehaviour
{
    private Vector3 pos;
    private SpriteRenderer spriteRenderer;
    public ItemSO item;

    public void Initialize (ItemSO item, Vector3 pos)
    {
        this.item = item;
        this.pos = pos;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite2D;
    }

    public void UpdatePosition(int screenWidth, int screenHeight){
        int width = screenWidth; // z jakiegoś managera faktyczne liczby później
        int height = screenHeight;
        float newX = Mathf.Lerp(0, width, pos.x);
        float newY = Mathf.Lerp(0, height, pos.y);
        transform.position = new Vector3(newX, newY, 0);
    }
}
