using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

public class SpritePreview : MonoBehaviour {
    [SerializeField] UIDocument document;
    [SerializeField] Camera previewCamera; // renders only the preview layer

    VisualElement element;
    [Inject] Camera gameCamera;
    RenderTexture rt;
    Vector2Int rtSize;

    void OnEnable() {
        element = document.rootVisualElement.Q("SettingsScalePreview");
        element.RegisterCallback<GeometryChangedEvent>(_ => Sync());
        Sync();
    }

    void OnDisable() {
        if (previewCamera != null)
            previewCamera.targetTexture = null;
        element.style.backgroundImage = null;
        Release();
    }

    // Panel points -> screen pixels. Root always fills the panel, so the
    // ratio of screen height to root height is the scale factor.
    float PanelScale() {
        var rootHeight = document.rootVisualElement.resolvedStyle.height;
        return rootHeight > 0f ? Screen.height / rootHeight : 1f;
    }

    void Sync() {
        var r = element.contentRect;
        if (r.width <= 0f || r.height <= 0f) return;

        var scale = PanelScale();
        var size = new Vector2Int(
            Mathf.Max(1, Mathf.RoundToInt(r.width * scale)),
            Mathf.Max(1, Mathf.RoundToInt(r.height * scale)));

        if (size != rtSize || rt == null) {
            Release();
            rtSize = size;

            var desc = new RenderTextureDescriptor(
                size.x,
                size.y,
                RenderTextureFormat.ARGB32,
                24) {
                depthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt,
                msaaSamples = 1,
                useMipMap = false,
                autoGenerateMips = false
            };

            rt = new(desc) { name = "SpritePreview" };
            rt.Create();

            previewCamera.targetTexture = rt;
            element.style.backgroundImage =
                new(Background.FromRenderTexture(rt));
        }

        // 1:1 density with the game camera.
        previewCamera.orthographic = true;
        previewCamera.orthographicSize =
            rtSize.y * gameCamera.orthographicSize / Screen.height;
    }

    void Release() {
        if (rt == null) return;
        rt.Release();
        Destroy(rt);
        rt = null;
        rtSize = default;
    }
}