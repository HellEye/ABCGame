using Reflex.Core;
using UnityEngine;

[ExecuteInEditMode]
public class DragAndDropSceneInstaller : MonoBehaviour, IInstaller {
    [Header("References")]
    [SerializeField] ScreenSizeManager screenSizeManager;

    [SerializeField] ItemSpawnerManager itemSpawnerManager;
    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] DropZoneUIController uiController;

    public void InstallBindings(ContainerBuilder builder) {
        builder.RegisterType<DropZoneItems>();
        builder.RegisterValue(screenSizeManager);
        builder.RegisterValue(itemSpawnerManager);
        builder.RegisterValue(gameManager);
        builder.RegisterValue(uiController);
    }
}