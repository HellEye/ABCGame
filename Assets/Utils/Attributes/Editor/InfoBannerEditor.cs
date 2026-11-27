using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public class InterfaceInspectorBase<T> : Editor where T : class {
    readonly Dictionary<string, object> _lastValues =
        new();

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var obj = target as T;
        if (obj == null) return;

        DrawInfoBanners(obj);
    }

    void DrawInfoBanners(T obj) {
        var properties = obj.GetType()
            .GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            );

        foreach (var prop in properties) {
            var attr = prop.GetCustomAttribute<InfoBannerAttribute>();
            if (attr == null) continue;

            var value = prop.GetValue(obj);
            var key = prop.Name;

            // Only redraw if value changed
            if (!_lastValues.TryGetValue(key, out var lastValue) ||
                !Equals(lastValue, value))
                _lastValues[key] = value;
            if (value is true)
                EditorGUILayout.HelpBox(
                    attr.Message,
                    MessageType.Warning
                );
        }
    }
}

[CustomEditor(typeof(LetterSetSO))]
public class LetterSetSOEditor : InterfaceInspectorBase<ISpawnableGroup> { }

[CustomEditor(typeof(ItemCategorySO))]
public class ItemCategorySOEditor : InterfaceInspectorBase<ISpawnableGroup> { }

[CustomEditor(typeof(ItemGroup))]
public class ItemGroupEditor : InterfaceInspectorBase<ISpawnableGroup> { }

[CustomEditor(typeof(ItemGroupFromCategory))]
public class ItemGroupFromCategoryEditor : InterfaceInspectorBase<ISpawnableGroup> { }

[CustomEditor(typeof(ItemGroupWithTargets))]
public class ItemGroupWithTargetsEditor : InterfaceInspectorBase<ISpawnableGroup> { }