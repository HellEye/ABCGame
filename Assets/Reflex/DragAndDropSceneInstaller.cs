using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

[ExecuteInEditMode]
public class DragAndDropSceneInstaller : MonoBehaviour, IInstaller {
    [Header("References")]
    [SerializeField] ScreenSizeManager screenSizeManager;

    [SerializeField] BackgroundController backgroundController;
    [SerializeField] DropZoneGameManager gameManager;

    [SerializeField] ItemSpawnerManager itemSpawnerManager;

    public void InstallBindings(ContainerBuilder builder) {
        builder.RegisterType<ItemContainer>(
            Lifetime.Singleton,
            Resolution.Eager,
            typeof(IRandomItemContainer),
            typeof(ItemContainer)
        );
        builder.RegisterValue(screenSizeManager);
        builder.RegisterValue(itemSpawnerManager);
        builder.RegisterValue(gameManager, new[] { typeof(IGameManager), typeof(DropZoneGameManager) });
        builder.RegisterValue(backgroundController);
    }
}