using System;
using System.Linq;
using Mono.Cecil.Cil;
using Reflex.Attributes;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

public class SettingsController : MonoBehaviour
{
    [Inject] UIDocument document;
    [Inject] MainMenuSettingsData settingsData;

    [SerializeField] GameObject spriteScalingReference;

    //string[] tabsNamesWithoutScaling;

    void Awake()
    {
        EnumDropdownBinding.RegisterConverter(MainMenuSettingsData.IntensityTextGetter);
        EnumDropdownBinding.RegisterConverter(MainMenuSettingsData.ColorblindTextGetter);
        var rootElement = document.rootVisualElement;
        //tabsNamesWithoutScaling[0] = "TabA";

        // settings popup
        Popup settingsPopup = rootElement.Q<Popup>("settings-popup");
        settingsPopup.dataSource = settingsData;
        settingsPopup.WithOpenButton(rootElement.Q<Button>("options"));
        settingsPopup.WithCloseButton(rootElement.Q<Button>("settings-close"));

        // settings button bindings
        settingsPopup.Q<Button>("settings-save").clicked += () => settingsData.Save();
        rootElement.Q<Button>("options").clicked += () =>
        {
            spriteScalingReference.SetActive(settingsPopup.Q<TabView>("settings-tabs").activeTab.name == "TabB");
        };
        settingsPopup.Q<Button>("settings-close").clicked += () =>
        {
            spriteScalingReference.SetActive(false);
            settingsData.Load();
        };
        settingsPopup.Q<TabView>("settings-tabs").activeTabChanged += (oldTab, newTab) =>
        {
            if (newTab?.name == "TabB")
            {
                spriteScalingReference.SetActive(true);
            }
            else if (oldTab?.name == "TabB")
            {
                spriteScalingReference.SetActive(false);
            }
            else
            {
                spriteScalingReference.SetActive(false);
            }
        };

        settingsPopup.Q<Button>("settings-reset").clicked += () => settingsData.Reset();

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
        SampleBinding();
    }

    [ContextMenu("Test Binding")]
    public void Test() => settingsData.VoVolume += 10;

    void SampleBinding() =>
        // Example of how to bind to settings data
        settingsData.propertyChanged += (sender, args) =>
        {
            // If you need a specific property, check for the name
            if (args.propertyName == nameof(settingsData.SoundVolume))
            {
                Debug.Log($"Sound volume changed to {settingsData.SoundVolume}");
            }
            // If you need to do something with the whole object, use the property bag
            else
            {
                var bag = PropertyBag.GetPropertyBag<MainMenuSettingsData>();
                var data = (MainMenuSettingsData)sender;
                // Don't use the bag collection with linq too often, it's slow, but this is a demo
                // Normally you'd use a foreach(var prop in bag.GetProperties())

                var value = bag
                    .GetProperties() // Gets all the properties
                                     // prop has Name and Get/Set value methods, check docs for more.
                                     // Here I'm just finding the property that matches the event
                    .First(prop =>
                        prop.Name ==
                        args.propertyName)
                    // You need to pass the data object, as the bag only holds the "shape" of the object, not the object itself
                    .GetValue(ref data);
                Debug.Log(
                    $"{args.propertyName} prop changed to {value}");
            }
        };
}