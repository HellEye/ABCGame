using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour {
    public int buttonsPerPage = 4;
    public int maxButtons = 12;
    [Inject] readonly MainMenuSettingsData settingsData;
    readonly Button[] slotButtons = new Button[4]; //need for new if you initialize onEnable?
    [Inject] UIDocument mainMenuDoc;
    VisualElement popupOverlay;
    VisualElement rootElement;

    int startIndex;

    void Awake() {
        rootElement = mainMenuDoc.rootVisualElement;
        for (var i = 0; i < buttonsPerPage; i++)
            slotButtons[i] =
                rootElement.Q<Button>($"slot-{i}"); //will throw a bug if it is different than uxml slot buttons number

        rootElement.Q<Button>("btn-left").clicked += () => ChangePage(-buttonsPerPage);
        rootElement.Q<Button>("btn-right").clicked += () => ChangePage(buttonsPerPage);


        UpdateUI();
    }

    void Start() => rootElement.FlattenTemplateContainers();

    void ChangePage(int step) {
        startIndex += step;

        if (startIndex >= maxButtons)
            startIndex = 0;
        else if (startIndex < 0) startIndex = maxButtons - buttonsPerPage;

        UpdateUI();
    }

    void UpdateUI() {
        for (var i = 0; i < buttonsPerPage; i++) slotButtons[i].text = (startIndex + i + 1).ToString();
    }
}