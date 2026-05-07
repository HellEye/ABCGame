using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] UIDocument mainMenuDoc;

    public int buttonsPerPage = 4;
    public int maxButtons = 12;
    public readonly MainMenuSettingsData settingsData = new();
    readonly Button[] slotButtons = new Button[4]; //need for new if you initialize onEnable?
    Dictionary<string, LocalizedDropdown<SettingsIntensity>> dropdowns = new();
    VisualElement popupOverlay;
    VisualElement rootElement;

    Popup settingsPopup;
    int startIndex;

    void Start() {
        EnumDropdownBinding.RegisterConverter(MainMenuSettingsData.IntensityTextGetter);
        rootElement = mainMenuDoc.rootVisualElement;
        settingsData.Load();
        for (var i = 0; i < buttonsPerPage; i++)
            slotButtons[i] =
                rootElement.Q<Button>($"slot-{i}"); //will throw a bug if it is different than uxml slot buttons number

        rootElement.Q<Button>("btn-left").clicked += () => ChangePage(-buttonsPerPage);
        rootElement.Q<Button>("btn-right").clicked += () => ChangePage(buttonsPerPage);

        settingsPopup = rootElement.Q<Popup>("settings-popup").WithOpenButton(rootElement.Q<Button>("options"));
        settingsPopup.WithCloseButton(settingsPopup.Q<Button>("settings-close"));
        rootElement.FlattenTemplateContainers();
        settingsPopup.dataSource = settingsData;
        settingsPopup.Q<Button>("settings-save").clicked += () => settingsData.Save();
        settingsPopup.Q<Button>("settings-close").clicked += () => {
            settingsPopup.IsOpen = false;
            settingsData.Load();
        };
        settingsPopup.Q<Button>("settings-reset").clicked += () => settingsData.Reset();

        // settingsPopup.Q<DropdownField>("motion-dropdown")
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("motion-dropdown"),
            MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("particle-dropdown"),
            MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("vibration-dropdown"),
            MainMenuSettingsData.IntensityTextGetter);
        UpdateUI();
        SampleBinding();
    }

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

    void SampleBinding() =>
        // Example of how to bind to settings data
        settingsData.propertyChanged += (sender, args) => {
            if (args.propertyName == nameof(settingsData.SoundVolume)) {
                Debug.Log($"Sound volume changed to {settingsData.SoundVolume}");
            }
            else {
                var bag = PropertyBag.GetPropertyBag<MainMenuSettingsData>();
                var data = (MainMenuSettingsData)sender;
                // Don't use the bag collection with linq too often, it's slow, but this is a demo
                Debug.Log(
                    $"{args.propertyName} prop changed to {bag.GetProperties().First(prop => prop.Name == args.propertyName).GetValue(ref data)}");
            }
        };
}