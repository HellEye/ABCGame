using System.Linq;
using Reflex.Attributes;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour {
    [SerializeField] GameObject spriteScalingReference;
    [Inject] UIDocument document;
    [Inject] MainMenuSettingsData settingsData;
    //string[] tabsNamesWithoutScaling;

    void Awake() {
        EnumDropdownBinding.RegisterConverter(MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.RegisterConverter(MainMenuSettingsData.ColorblindTextGetter);
        var rootElement = document.rootVisualElement;
        //tabsNamesWithoutScaling[0] = "TabA";
        var tabView = rootElement.Q<TabView>("settings-tabs");
        // settings popup
        var settingsPopup = rootElement.Q<Popup>("settings-popup");
        settingsPopup.dataSource = settingsData;
        settingsPopup.WithOpenButton(rootElement.Q<Button>("options"));
        settingsPopup.WithCloseButton(rootElement.Q<Button>("settings-close"));

        // settings button bindings
        rootElement.Q<Button>("options").clicked += () => {
            spriteScalingReference.SetActive(settingsPopup.Q<TabView>("settings-tabs").activeTab.name == "TabB");
        };
        settingsPopup.Q<Button>("settings-close").clicked += () =>
        {
            spriteScalingReference.SetActive(false);
        };
        settingsPopup.Q<TabView>("settings-tabs").activeTabChanged += (oldTab, newTab) => {
            if (newTab?.name == "TabB")
                spriteScalingReference.SetActive(true);
            else if (oldTab?.name == "TabB")
                spriteScalingReference.SetActive(false);
            else
                spriteScalingReference.SetActive(false);
        };

        settingsPopup.Q<Button>("settings-reset").clicked += () =>
        {
            settingsData.Reset();
            settingsData.Save();
        };

        // dropdown bindings
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("motion-dropdown"),
            MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("particle-dropdown"),
            MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("vibration-dropdown"),
            MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.SetChoices(
            settingsPopup.Q<DropdownField>("colorblind-dropdown"),
            MainMenuSettingsData.ColorblindTextGetter);
        SetupAutoSave();
    }

    [ContextMenu("Test Binding")]
    public void Test() => settingsData.VoVolume += 10;

    void SetupAutoSave()
    {
        settingsData.propertyChanged += (sender, args) =>
        {
            settingsData.Save();

            Debug.Log($"Settings changed: {args.propertyName} - autosaved.");
        };
    }
}