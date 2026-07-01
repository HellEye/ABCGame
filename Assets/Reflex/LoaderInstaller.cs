using System;
using Reflex.Core;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class LoaderUIDocument
{
    [SerializeField] public UIDocument value;
    public LoaderUIDocument(UIDocument value) => this.value = value;
}

public class LoaderInstaller : MonoBehaviour, IInstaller
{
    [Header("Settings")]
    [SerializeField] MinigameRegistry minigameRegistry;

    [SerializeField] ItemRegistry itemRegistry;
    [SerializeField] ExcludeItemsSO excludeItems;

    [Header("References")]
    [SerializeField] UIDocument uiDocument;

    [SerializeField] GameLoader gameLoader;
    [SerializeField] LoaderUIController loaderUIController;
    [SerializeField] Camera cam;

    [SerializeField] ScreenSizeManager screenSizeManager;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue(cam);
        builder.RegisterValue(minigameRegistry);
        builder.RegisterValue(new LoaderUIDocument(uiDocument));
        builder.RegisterValue(loaderUIController);
        builder.RegisterValue(gameLoader);
        builder.RegisterValue(excludeItems);
        builder.RegisterValue(itemRegistry);
        builder.RegisterType<DifficultyHolder>();
        builder.RegisterValue(screenSizeManager);
    }
}