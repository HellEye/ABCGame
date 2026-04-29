using UnityEngine;
using UnityEngine.UIElements;

public class PopupTestUIController : MonoBehaviour {
    Popup popup;
    VisualElement root;

    void Start() {
        root = GetComponent<UIDocument>().rootVisualElement;


        var openButton = root.Q<Button>("openPopup");
        // can chain multiple WithOpenButton/WithCloseButton calls
        // can also inline the root.Q call
        popup = root.Q<Popup>("testPopup")
            .WithOpenButton(openButton)
            .WithCloseButton(root.Q<Button>("closePopupButton"));

        // Alternative to "WithOpenButton" for finer control
        //openButton.clicked += () => popup.IsOpen = true;
    }

    [ContextMenu("Open Popup")]
    public void OpenPopup() => popup.IsOpen = true;

    [ContextMenu("Close Popup")]
    public void ClosePopup() => popup.IsOpen = false;
}