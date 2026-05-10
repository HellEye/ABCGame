#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class EditorBootstrapHelper {
    const string LOADER_SCENE_NAME = "Loader";

    static EditorBootstrapHelper() {
        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(
                $"Assets/Scenes/{LOADER_SCENE_NAME}.unity");

        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state) {
        if (state != PlayModeStateChange.ExitingEditMode) return;
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != LOADER_SCENE_NAME) EditorPrefs.SetString("LastEditScene", activeScene.name);
    }
}
#endif