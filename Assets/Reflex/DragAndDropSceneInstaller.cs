using Reflex.Core;
using UnityEngine;

[ExecuteInEditMode]
public class DragAndDropSceneInstaller : MonoBehaviour, IInstaller {
    [Header("Settings")]
    [SerializeField] DropZoneGameDifficulty difficulty;

    [SerializeField] InterfaceReference<ISpawnableGroup> spawnableGroup;

    [Header("References")]
    [SerializeField] Camera cam;

    [SerializeField] ScreenSizeManager screenSizeManager;
    [SerializeField] ItemSpawnerManager itemSpawnerManager;
    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] DropZoneUIController uiController;

    public void InstallBindings(ContainerBuilder builder) {
        builder.RegisterValue(cam);
        builder.RegisterValue(difficulty);
        builder.RegisterValue(spawnableGroup.Value, new[] { typeof(ISpawnableGroup) });
        builder.RegisterType<DropZoneItems>();
        builder.RegisterValue(screenSizeManager);
        builder.RegisterValue(itemSpawnerManager);
        builder.RegisterValue(gameManager);
        builder.RegisterValue(uiController);
    }
}