using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameLoader : MonoBehaviour {
    [SerializeField] SceneReference mainMenuScene;
    [SerializeField] SceneReference dragAndDropScene;

    readonly Dictionary<SceneReference, SceneLifecycle> lifecycle = new();
    Container container;
    SceneReference currentScene;
    [Inject] DifficultyHolder difficultyHolder;
    bool ready;

    [Inject] LoaderUIController uiController;
    bool isCurrentSceneValid => currentScene != null && currentScene.State != SceneReferenceState.Unsafe;

    void Awake() {
        container = gameObject.scene.GetSceneContainer();
        lifecycle.Add(mainMenuScene, new(container));
        lifecycle.Add(dragAndDropScene, new(container));
        Debug.Log("Scene count: " + SceneManager.sceneCount);

        // if we open a different scene, reload the current one for ordering
        // Editor only, since the full game will just start on the loader scene in the main menu
        // see EditorBootstrapHelper for always loading this scene
#if UNITY_EDITOR
        var lastScene = EditorPrefs.GetString("LastEditScene", "");
        if (!ready && !string.IsNullOrEmpty(lastScene) && lastScene != SceneManager.GetActiveScene().name) {
            var sceneToLoad = GetSceneByName(lastScene);
            if (sceneToLoad != null) {
                // TODO need to load this differently, going through the LoadGameplayScene
                EditorPrefs.DeleteKey("LastEditScene");
                currentScene = GetSceneByName(lastScene);
                if (currentScene.Name == "MainMenu") LoadMainMenu();
                else
                    Debug.LogWarning("Loading gameplay scene not supported yet... skipping");
                ready = true;
            }
            else {
                Debug.LogError($"Scene {lastScene} not found in the scene list");
            }
        }
#endif

        // starting from loader scene, go to main menu
        var activeScene = SceneManager.GetActiveScene();
        if (!ready && activeScene.name == gameObject.scene.name) {
            LoadScene(mainMenuScene).Forget();
            ready = true;
        }
    }

    SceneReference GetSceneByName(string name) {
        foreach (var kvp in lifecycle)
            if (kvp.Key.Name == name)
                return kvp.Key;

        return null;
    }

    public void ReloadCurrentGameplayScene() => LoadGameplaySceneFromHolder().Forget();

    public void LoadMainMenu() => LoadScene(mainMenuScene).Forget();

    public async UniTask LoadGameplaySceneFromHolder() {
        var scene = difficultyHolder.selectedScene;
        var difficulty = difficultyHolder.selectedDifficulty;
        lifecycle[scene].OnContainerBuilt += SetupDifficulty;
        await LoadScene(scene);
        lifecycle[scene].OnContainerBuilt -= SetupDifficulty;
        return;

        void SetupDifficulty(Scene scene, ContainerBuilder builder) =>
            builder.RegisterValue(difficulty, new[] { difficulty.type });
    }


    async UniTask<Scene> LoadScene(SceneReference scene) {
        await uiController.FadeOut();
        await UnloadCurrentScene();
        lifecycle[scene].OnBeforeLoad.Invoke();
        await SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
        var loadedScene = SceneManager.GetSceneByName(scene.Name);
        SceneManager.SetActiveScene(loadedScene);
        lifecycle[scene].OnSceneLoaded.Invoke(loadedScene);
        currentScene = scene;

        await uiController.FadeIn();
        return loadedScene;
    }

    async UniTask UnloadCurrentScene() {
        Debug.Log(
            !isCurrentSceneValid ? "No current scene to unload" : $"Unloading current scene: {currentScene.Name}");
        if (!isCurrentSceneValid) return;
        var scene = SceneManager.GetSceneByName(currentScene.Name);
        if (!scene.isLoaded) return;
        lifecycle[currentScene].OnSceneUnloaded.Invoke(scene);
        await SceneManager.UnloadSceneAsync(currentScene.Name);
    }
}

public class SceneLifecycle {
    readonly Container parentContainer;
    public Action OnBeforeLoad;
    public Action<Scene, ContainerBuilder> OnContainerBuilt;
    public Action<Scene> OnSceneLoaded;
    public Action<Scene> OnSceneUnloaded;

    public SceneLifecycle(Container parentContainer) {
        this.parentContainer = parentContainer;
        // Default lifecycle events for reflex scoping
        OnBeforeLoad += () => {
            ContainerScope.OnSceneContainerBuilding += ContainerBuiltCallback;
        };
        OnSceneLoaded += _ => {
            ContainerScope.OnSceneContainerBuilding -= ContainerBuiltCallback;
        };
        OnSceneUnloaded += _ => { };
        OnContainerBuilt += (_, builder) => {
            builder.SetParent(parentContainer);
        };
    }

    void ContainerBuiltCallback(Scene scene, ContainerBuilder builder) => OnContainerBuilt?.Invoke(scene, builder);
}