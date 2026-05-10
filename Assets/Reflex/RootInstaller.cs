using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

/// <summary>
///     This is the root installer.
///     It is responsible for registering all the singletons accessible across all scenes
/// </summary>
public class RootInstaller : MonoBehaviour, IInstaller {
    public void InstallBindings(ContainerBuilder builder) {
        // create the main menu settings object.
        // register value to be accessible from anywhere.
        // I'm using register factory, as we need to manually call the Load method.
        // This could be done inline and registered with RegisterValue,
        // But it's more semantically correct to use a function.
        builder.RegisterFactory(
            _ => {
                var mainMenuSettings = new MainMenuSettingsData();
                mainMenuSettings.Load();
                return mainMenuSettings;
            },
            // Singleton means one will be created and shared across the whole application.
            // Transient means a new one will be created for each injection.
            // Scoped means a new one will be created for each scope (e.g., scene).
            // Most of the time we want a singleton.
            Lifetime.Singleton,
            // Eager means it will be created and initialized immediately.
            // Lazy means it will be created and initialized when first requested.
            Resolution.Eager);
        // We could register type if we don't have anything to initialize,
        // But here we need to call the Load method, and it doesn't work from constructors.
        // builder.RegisterType(typeof(MainMenuSettingsData), Lifetime.Singleton, Resolution.Eager);

        builder.RegisterType<LoaderContainer>();
    }
}