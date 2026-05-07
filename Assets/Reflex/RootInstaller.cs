using Reflex.Core;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller {
    public void InstallBindings(ContainerBuilder builder) {
        var mainMenuSettings = new MainMenuSettingsData();
        mainMenuSettings.Load();
        builder.RegisterValue(mainMenuSettings);
    }
}