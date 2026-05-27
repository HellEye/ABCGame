using Reflex.Core;
using UnityEngine;

[ExecuteInEditMode]
public class DragAndDropSceneInstaller : MonoBehaviour, IInstaller
{
    [Header("Settings")]
    [SerializeField] InterfaceReference<ISpawnableGroup> spawnableGroup;

    [Header("References")]

    [SerializeField] ItemSpawnerManager itemSpawnerManager;
    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] DropZoneUIController uiController;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue(spawnableGroup.Value, new[] { typeof(ISpawnableGroup) });
        builder.RegisterType<DropZoneItems>();
        builder.RegisterValue(itemSpawnerManager);
        builder.RegisterValue(gameManager);
        builder.RegisterValue(uiController);
    }
}