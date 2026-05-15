using Cysharp.Threading.Tasks;
using LitMotion;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class LoaderUIController : MonoBehaviour {
    [SerializeField] float fadeTime = 0.25f;
    [Inject] LoaderUIDocument document;
    VisualElement overlay;
    VisualElement root;

    void Awake() {
        root = document.value.rootVisualElement;
        overlay = root.Q<VisualElement>("loader-overlay");
    }

    public async UniTask FadeOut() {
        root.style.display = DisplayStyle.Flex;
        if (root.style.opacity != 0) return;
        await LMotion.Create(0f, 1f, fadeTime)
            .WithEase(Ease.Linear)
            .Bind(
                overlay.style,
                (v, style) => {
                    style.opacity = v;
                })
            .AddTo(this)
            .ToUniTask();
    }

    public async UniTask FadeIn() =>
        await LMotion.Create(1f, 0f, fadeTime)
            .WithEase(Ease.Linear)
            .WithOnComplete(() => root.style.display = DisplayStyle.None)
            .Bind(
                overlay.style,
                (v, style) => {
                    style.opacity = v;
                })
            .AddTo(this)
            .ToUniTask();
}