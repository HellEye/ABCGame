#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
<<<<<<< HEAD
public static class EditorBootstrapHelper {
    const string LOADER_SCENE_PATH = "Assets/Scenes/Loader.unity";
    static readonly string[] IGNORED_SCENES = { "UI Test", "PuzzleGameplay" };

    static EditorBootstrapHelper() {
=======
public class EditorBootstrapHelper {
    const string LOADER_SCENE_NAME = "Loader";
    static readonly string[] IGNORED_SCENES = { "UI Test", "PuzzleGameplay" };

    static EditorBootstrapHelper() {
        var activeScene = EditorSceneManager.GetActiveScene();
        Debug.Log("Active scene: " + activeScene.path + " " + activeScene.name);
        if (IGNORED_SCENES.Contains(activeScene.name)) {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            return;
        }

        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(
                $"Assets/Scenes/{LOADER_SCENE_NAME}.unity");

>>>>>>> 3501407 (Puzzle game proof of concept)
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