using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

[UxmlElement("Popup")]
public partial class Popup : VisualElement {
    static StyleSheet popupStyleSheet;
    readonly VisualElement wrapper;
    bool isOpen;

    public Popup() {
        // create the wrapper element
        wrapper = new() {
            name = "wrapper"
        };
        wrapper.AddToClassList("popup-wrapper");
        // have to use hierarchy.Add instead of Add because we're overriding contentContainer
        hierarchy.Add(wrapper);

        // treat this element as the backdrop
        AddToClassList("popup-backdrop");

        // close the popup when the backdrop is clicked
        RegisterCallback<PointerDownEvent>(OnBackdropClick);

        // load styles asynchronously
        LoadStyleSheet().Forget();
    }

    public override VisualElement contentContainer => wrapper;

    [UxmlAttribute]
    public bool IsOpen {
        get => isOpen;
        set {
            if (isOpen == value) return;
            isOpen = value;
            OnOpenChange(value);
        }
    }

    public async UniTask LoadStyleSheet() {
        // try the cached styles first if already loaded, otherwise load via addressables
        var styleSheet = popupStyleSheet;
        if (styleSheet == null) {
            styleSheet = await Addressables.LoadAssetAsync<StyleSheet>("UIElements/Popup/uss").ToUniTask();
            popupStyleSheet = styleSheet;
        }

        if (styleSheet == null) return;
        styleSheets.Add(styleSheet);
    }

    void OnBackdropClick(PointerDownEvent evt) {
        // only close the popup if the click was on the backdrop specifically
        if (evt.target == this)
            IsOpen = false;
    }

    public Popup WithOpenButton(Button button) {
        button.clicked += () => IsOpen = true;
        return this;
    }

    public Popup WithCloseButton(Button button) {
        button.clicked += () => IsOpen = false;
        return this;
    }

    void OnOpenChange(bool value) => EnableInClassList("show", value);
}