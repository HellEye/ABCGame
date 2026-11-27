using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
#endif

public class GameLoader : MonoBehaviour {
    [SerializeField] SceneReference mainMenuScene;
    [SerializeField] SceneReference dragAndDropScene;
    [Inject] BackgroundStore backgroundStore;
    Container container;

    SceneLifecycle currentLifecycle;
    SceneReference currentScene;
    [Inject] DifficultyHolder difficultyHolder;
    [Inject] ItemRegistry itemRegistry;
    bool ready;
    [Inject] TtsState ttsState;
    [Inject] IEnumerable<ITextToSpeechWarmup> ttsWarmups;

    [Inject] LoaderUIController uiController;
    bool isCurrentSceneValid => currentScene != null && currentScene.State != SceneReferenceState.Unsafe;

    void Awake() {
        container = gameObject.scene.GetSceneContainer();
        WarmupTts().Forget();

        // starting from loader scene, go to main menu
        var activeScene = SceneManager.GetActiveScene();
        if (!ready && activeScene.name == gameObject.scene.name) {
            LoadScene(mainMenuScene, CreateLifecycle()).Forget();
            ready = true;
        }
    }

    public async UniTaskVoid WarmupTts() {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        try {
            // TODO store when this is complete to unlock any buttons etc.
            Debug.Log("TTS count: " + ttsWarmups.Count());
            await UniTask.WhenAll(ttsWarmups.Select(tts => tts.WarmupAsync(ct)));
            ttsState.Available = true;
        }
        catch (Exception e) {
            Debug.LogException(e);
            cts.Cancel();
        }
        finally {
            stopwatch.Stop();
            Debug.Log($"TTS warmup took {stopwatch.ElapsedMilliseconds} ms");
        }
    }

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
            var background = container.Resolve<BackgroundController>();
            handles.Add(backgroundStore.LoadNextBackground(background.backgrounds, difficultyHolder.selectedMapping));
        }

        async Task EnsureLoad() => await Task.WhenAll(handles.Select(h => h.Task));


        void UnloadSprites(Scene scene) {
            foreach (var handle in handles) AssetReferenceExtensions.Release(handle);
        }

        void SetupDifficulty(Scene scene, ContainerBuilder builder) {
            builder.RegisterValue(difficulty, new[] { typeof(IDifficulty), difficulty.type });

            var spawnableGroupsForDifficulty = itemRegistry.GetGroupsFor(difficulty).ToList();
            var spawnableGroup = spawnableGroupsForDifficulty.PickRandom();
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