using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class LetterRenderer : MonoBehaviour, IElementRenderer {
    [SerializeField] UIDocument document;

    public UniTask Initialize(IElement element) {
        if (element is not Letter letter) return UniTask.CompletedTask;
        var label = document.rootVisualElement.Q<Label>();
        label.text = letter.letter;
        return UniTask.CompletedTask;
    }
}