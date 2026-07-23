#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class EditorBootstrapHelper {
    const string LOADER_SCENE_NAME = "Loader";
    static readonly string[] IGNORED_SCENES = { "UI Test" };

    static EditorBootstrapHelper() {
        var activeScene = EditorSceneManager.GetActiveScene();
        if (IGNORED_SCENES.Contains(activeScene.name)) {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            return;
        }

        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(
                $"Assets/Scenes/{LOADER_SCENE_NAME}.unity");

        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state) {
        if (state != PlayModeStateChange.ExitingEditMode) return;
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != LOADER_SCENE_NAME && !IGNORED_SCENES.Contains(activeScene.name))
            EditorPrefs.SetString("LastEditScene", activeScene.name);
    }
}
#endif