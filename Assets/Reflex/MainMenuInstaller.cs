using Reflex.Core;
using UnityEngine;

public class MainMenuInstaller : MonoBehaviour, IInstaller {
    [SerializeField] MainMenuManager mainMenuManager;

    public void InstallBindings(ContainerBuilder builder) => builder.RegisterValue(mainMenuManager);
}