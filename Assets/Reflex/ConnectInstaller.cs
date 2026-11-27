using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

public class ConnectInstaller : MonoBehaviour, IInstaller {
    [SerializeField] ScreenSizeManager screenSizeManager;
    [SerializeField] Camera cam;
    [SerializeField] ConnectSpawner connectSpawner;
    [SerializeField] BackgroundController backgroundController;
    [SerializeField] ConnectGameManager gameManager;

    public void InstallBindings(ContainerBuilder builder) {
        builder.RegisterType<ItemContainer>(
            Lifetime.Singleton,
            Resolution.Eager,
            typeof(IRandomItemContainer),
            typeof(ItemContainer));
        builder.RegisterValue(screenSizeManager);
        if (builder.HasBinding(typeof(Camera))) {
            Debug.Log("Camera already registered");
            cam.gameObject.SetActive(false);
        }
        else {
            builder.RegisterValue(cam);
        }

        builder.RegisterValue(connectSpawner);
        builder.RegisterValue(backgroundController);
        builder.RegisterValue(gameManager, typeof(ConnectGameManager), typeof(IGameManager));
    }
}