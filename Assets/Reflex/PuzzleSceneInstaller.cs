using Reflex.Core;
using UnityEngine;

public class PuzzleSceneInstaller : MonoBehaviour, IInstaller {
    [SerializeField] PuzzleManager puzzleManager;

    public void InstallBindings(ContainerBuilder builder) => builder.RegisterValue(puzzleManager);
}