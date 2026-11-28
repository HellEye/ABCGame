#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorBootstrapHelper {
    const string LOADER_SCENE_PATH = "Assets/Scenes/Loader.unity";
    static readonly string[] IGNORED_SCENES = { "UI Test", "PuzzleGameplay" };

    static EditorBootstrapHelper() {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        UpdatePlayModeStartScene();
    }

    static void OnPlayModeChanged(PlayModeStateChange state) {
        // Runs right before play mode starts, so the decision is made fresh every time.
        if (state == PlayModeStateChange.ExitingEditMode)
            UpdatePlayModeStartScene();
    }

    static void UpdatePlayModeStartScene() {
        var activeScene = EditorSceneManager.GetActiveScene();

        // Ignored scenes play themselves; everything else starts from the Loader.
        var path = IGNORED_SCENES.Contains(activeScene.name) ? activeScene.path : LOADER_SCENE_PATH;
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
    }
}
#endif