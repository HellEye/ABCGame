using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
#endif

public class GameLoader : MonoBehaviour {
    [SerializeField] SceneReference mainMenuScene;
    [SerializeField] SceneReference dragAndDropScene;
    Container container;

    SceneLifecycle currentLifecycle;
    SceneReference currentScene;
    [Inject] DifficultyHolder difficultyHolder;
    [Inject] ItemRegistry itemRegistry;
    bool ready;

    [Inject] LoaderUIController uiController;
    bool isCurrentSceneValid => currentScene != null && currentScene.State != SceneReferenceState.Unsafe;

    void Awake() {
        container = gameObject.scene.GetSceneContainer();
        Debug.Log("Scene count: " + SceneManager.sceneCount);

        // if we open a different scene, reload the current one for ordering
        // Editor only, since the full game will just start on the loader scene in the main menu
        // see EditorBootstrapHelper for always loading this scene
#if UNITY_EDITOR
        // LoadMainMenu();
        // TODO need to load this differently, going through the LoadGameplayScene
        /*var lastScene = EditorPrefs.GetString("LastEditScene", "");
        if (!ready && !string.IsNullOrEmpty(lastScene) && lastScene != SceneManager.GetActiveScene().name) {
            var sceneToLoad = GetSceneByName(lastScene);
            if (sceneToLoad != null) {
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
        }*/
#endif

        // starting from loader scene, go to main menu
        var activeScene = SceneManager.GetActiveScene();
        if (!ready && activeScene.name == gameObject.scene.name) {
            LoadScene(mainMenuScene, CreateLifecycle()).Forget();
            ready = true;
        }
    }


    /*SceneReference GetSceneByName(string name) {
        foreach (var kvp in lifecycle)
            if (kvp.Key.Name == name)
                return kvp.Key;

        return null;
    }*/
    SceneLifecycle CreateLifecycle() => new(container);

    public void ReloadCurrentGameplayScene() => LoadGameplaySceneFromHolder().Forget();

    public void LoadMainMenu() => LoadScene(mainMenuScene, CreateLifecycle()).Forget();

    public async UniTask LoadGameplaySceneFromHolder() {
        var scene = difficultyHolder.selectedScene;
        var difficulty = difficultyHolder.selectedDifficulty;
        var handles = new List<AsyncOperationHandle<Sprite>>();
        var lifecycle = CreateLifecycle();
        lifecycle.OnContainerBuilt += SetupDifficulty;
        lifecycle.OnSceneLoaded += LoadSprites;

        await LoadScene(scene, lifecycle, async _ => await EnsureLoad());

        lifecycle.OnContainerBuilt -= SetupDifficulty;
        lifecycle.OnSceneLoaded -= LoadSprites;

        lifecycle.OnSceneUnloaded += UnloadSprites;
        return;

        void LoadSprites(Scene scene) {
            var container = scene.GetSceneContainer();
            var items = container.Resolve<IRandomItemContainer>();
            foreach (var item in items.GetAllItems().Distinct())
                if (item is ItemSO itemSo)
                    handles.Add(itemSo.sprite.Load());
        }

        async Task EnsureLoad() => await Task.WhenAll(handles.Select(h => h.Task));


        void UnloadSprites(Scene scene) {
            foreach (var handle in handles) AssetReferenceExtensions.Release(handle);
        }

        void SetupDifficulty(Scene scene, ContainerBuilder builder) {
            builder.RegisterValue(difficulty, new[] { difficulty.type });

            var spawnableGroupsForDifficulty = itemRegistry.GetGroupsFor(difficulty).ToList();
            var spawnableGroup = spawnableGroupsForDifficulty[Random.Range(0, spawnableGroupsForDifficulty.Count)];
            Debug.Log($"SpawnableGroup: {spawnableGroup.Title}");
            builder.RegisterValue(spawnableGroup, new[] { typeof(ISpawnableGroup) });
        }
    }


    async UniTask<Scene> LoadScene(SceneReference scene, SceneLifecycle lifecycle,
        Func<Scene, Task> beforeFadeIn = null) {
        await uiController.FadeOut();

        await UnloadCurrentScene();
        currentLifecycle = lifecycle;
        lifecycle.OnBeforeLoad.Invoke();

        await SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
        var loadedScene = SceneManager.GetSceneByName(scene.Name);
        SceneManager.SetActiveScene(loadedScene);

        lifecycle.OnSceneLoaded.Invoke(loadedScene);
        currentScene = scene;

        if (beforeFadeIn != null) await beforeFadeIn(loadedScene);

        await uiController.FadeIn();
        return loadedScene;
    }

    async UniTask UnloadCurrentScene() {
        Debug.Log(
            !isCurrentSceneValid ? "No current scene to unload" : $"Unloading current scene: {currentScene.Name}");
        if (!isCurrentSceneValid) return;
        var scene = SceneManager.GetSceneByName(currentScene.Name);
        if (!scene.isLoaded) return;
        currentLifecycle?.OnSceneUnloaded.Invoke(scene);
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