using UnityEngine;

public class OutlineController : MonoBehaviour {
    [SerializeField] SpriteRenderer spriteRenderer;


    bool outlineEnabled;

    public bool OutlineEnabled {
        get => outlineEnabled;
        set {
            outlineEnabled = value;
            UpdateOutline(value);
        }
    }


    void UpdateOutline(bool isEnabled) {
        if (!spriteRenderer) return;
        // renderer.material creates a per-renderer instance,
        // which is needed because keywords aren't per-renderer.
        var mat = spriteRenderer.material;
        if (isEnabled) mat.EnableKeyword("_OUTLINE_ON");
        else mat.DisableKeyword("_OUTLINE_ON");
    }
}