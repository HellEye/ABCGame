using Reflex.Core;
using UnityEngine;

[ExecuteInEditMode]
public class DragAndDropSceneInstaller : MonoBehaviour, IInstaller {
    [SerializeField] Camera cam;
    [SerializeField] ScreenSizeManager screenSizeManager;
    [SerializeField] ItemSpawnerManager itemSpawnerManager;
    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] DropZoneUIController uiController;
    [SerializeField] DropZoneGameDifficulty difficulty;

    public void InstallBindings(ContainerBuilder builder) {
        builder.RegisterValue(cam);
        builder.RegisterValue(screenSizeManager);
        builder.RegisterValue(itemSpawnerManager);
        builder.RegisterValue(gameManager);
        builder.RegisterValue(uiController);
        builder.RegisterValue(difficulty);
    }
}