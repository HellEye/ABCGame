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
    [Inject] LoaderContainer loaderContainer;
    bool ready;

    [Inject] LoaderUIController uiController;
    bool isCurrentSceneValid => currentScene != null && currentScene.State != SceneReferenceState.Unsafe;

    void Awake() {
        container = gameObject.scene.GetSceneContainer();
        loaderContainer.container = container;
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
                EditorPrefs.DeleteKey("LastEditScene");
                currentScene = GetSceneByName(lastScene);
                ReloadCurrentScene().Forget();
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

    async UniTaskVoid ReloadCurrentScene() {
        if (!isCurrentSceneValid) return;
        await uiController.FadeOut();
        await UnloadCurrentScene();
        lifecycle[currentScene].OnBeforeLoad.Invoke();
        await SceneManager.LoadSceneAsync(currentScene.Name, LoadSceneMode.Additive);
        var loadedScene = SceneManager.GetSceneByName(currentScene.Name);
        SceneManager.SetActiveScene(loadedScene);
        lifecycle[currentScene].OnSceneLoaded.Invoke(loadedScene);
        await uiController.FadeIn();
    }

    async UniTaskVoid LoadScene(SceneReference scene) {
        await uiController.FadeOut();
        await UnloadCurrentScene();
        lifecycle[scene].OnBeforeLoad.Invoke();
        await SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
        var loadedScene = SceneManager.GetSceneByName(scene.Name);
        SceneManager.SetActiveScene(loadedScene);
        lifecycle[scene].OnSceneLoaded.Invoke(loadedScene);
        currentScene = scene;

        await uiController.FadeIn();
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
    }

    public SceneLifecycle AddOnBeforeLoad(Action action) {
        OnBeforeLoad += action;
        return this;
    }

    public SceneLifecycle AddOnSceneLoaded(Action<Scene> action) {
        OnSceneLoaded += action;
        return this;
    }

    public SceneLifecycle AddOnSceneUnloaded(Action<Scene> action) {
        OnSceneUnloaded += action;
        return this;
    }

    void ContainerBuiltCallback(Scene scene, ContainerBuilder builder) => builder.SetParent(parentContainer);
}