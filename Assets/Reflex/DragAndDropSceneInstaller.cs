using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

[ExecuteInEditMode]
public class DragAndDropSceneInstaller : MonoBehaviour, IInstaller {
    [Header("References")]
    [SerializeField] ScreenSizeManager screenSizeManager;

    [SerializeField] ItemSpawnerManager itemSpawnerManager;
    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] DropZoneUIController uiController;

    public void InstallBindings(ContainerBuilder builder) {
        builder.RegisterType(
            typeof(DropZoneItems),
            new[] { typeof(IRandomItemContainer), typeof(DropZoneItems) },
            Lifetime.Singleton,
            Resolution.Eager);
        builder.RegisterValue(screenSizeManager);
        builder.RegisterValue(itemSpawnerManager);
        builder.RegisterValue(gameManager);
        builder.RegisterValue(uiController);
    }
}