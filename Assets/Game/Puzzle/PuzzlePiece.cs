using UnityEngine;

public class PuzzlePiece : MonoBehaviour {
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] BoxCollider2D boxCollider;

    public void Init(Material material, PuzzlePieceData data) {
        meshFilter.mesh = data.Mesh;
        meshRenderer.material = material;
        boxCollider.size = new(data.Bounds.size.x, data.Bounds.size.y);
    }
}