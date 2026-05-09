using Reflex.Core;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
///     This is the installer for the main menu scene
/// </summary>
public class MainMenuInstaller : MonoBehaviour, IInstaller {
    // Add any monobehaviour dependencies here
    [SerializeField] MainMenuManager mainMenuManager;
    [SerializeField] UIDocument mainMenuDoc;
    [SerializeField] SettingsController settingsController;

    public void InstallBindings(ContainerBuilder builder) {
        // register them all here
        builder.RegisterValue(mainMenuDoc);
        builder.RegisterValue(mainMenuManager);
        builder.RegisterValue(settingsController);
    }
}