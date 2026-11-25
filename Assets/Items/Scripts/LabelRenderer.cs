using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public enum Side {
    Left,
    Right
}

public class LabelRenderer : MonoBehaviour, IElementRenderer {
    [SerializeField] UIDocument document;
    [SerializeField] Side side;
    [SerializeField] NotchedLabelMode mode;
    [SerializeField] bool tts;
    [SerializeField] InterfaceReference<ISelectEvents> selectEvents;
    IElement element;
    AsyncOperationHandle<Sprite> handle;
    NotchedLabel label;
    [Inject] TtsAudio textToSpeech;
    [Inject] TtsState ttsState;

    public bool Highlighted {
        get => label.ClassListContains("selected");
        set => label.EnableInClassList("selected", value);
    }

    void Awake() => label = document.rootVisualElement.Q<NotchedLabel>();

    void OnEnable() {
        if (selectEvents.Value == null) return;
        selectEvents.Value.OnSelected += OnSelect;
        selectEvents.Value.OnDeselected += OnDeselect;
    }

    void OnDisable() {
        if (selectEvents.Value == null) return;
        selectEvents.Value.OnSelected -= OnSelect;
        selectEvents.Value.OnDeselected -= OnDeselect;
    }

    public void OnDestroy() {
        if (handle.IsValid()) handle.Release();
    }

    public async UniTask Initialize(IElement element) {
        this.element = element;
        if (mode == NotchedLabelMode.Text) {
            label.Mode = NotchedLabelMode.Text;
            label.Text = element.DisplayNameSingular;
        }
        else {
            label.Mode = NotchedLabelMode.Icon;
            handle = element.Icon.LoadAssetAsync<Sprite>();
            label.IconTexture = (await handle).texture;
        }

        if (side == Side.Right) label.AddToClassList("notch-right-side");

        if (tts) {
            label.Clicked += OnTtsClick;
            label.ActionButton.SetEnabled(false);
            if (ttsState.Available) {
                label.ActionButton.SetEnabled(true);
            }
            else {
                label.ActionButton.SetEnabled(false);
                ttsState.OnAvailableChanged += available => {
                    label.ActionButton.SetEnabled(available);
                };
            }
        }
    }

    void OnSelect() => Highlighted = true;

    void OnDeselect() => Highlighted = false;


    void OnTtsClick() => textToSpeech.Play(element.DisplayNameSingular);
}