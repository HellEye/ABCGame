using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelRenderer : MonoBehaviour, IElementRenderer {
    [SerializeField] UIDocument document;

    public UniTask Initialize(IElement element) {
        var label = document.rootVisualElement.Q<Label>();
        label.text = element.TargetDisplayName;
        return UniTask.CompletedTask;
    }
}