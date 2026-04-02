using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxRect))]
public class MinMaxRectDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label
    )
    {
        return EditorGUIUtility.singleLineHeight * 3f +
               EditorGUIUtility.standardVerticalSpacing * 2f;
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label
    )
    {
        var minProp = property.FindPropertyRelative("min");
        var maxProp = property.FindPropertyRelative("max");

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        var lineHeight = EditorGUIUtility.singleLineHeight;
        var spacing = EditorGUIUtility.standardVerticalSpacing;

        var row1 = new Rect(position.x, position.y, position.width, lineHeight);
        var row2 = new Rect(
            position.x,
            position.y + lineHeight + spacing,
            position.width,
            lineHeight
        );
        var row3 = new Rect(
            position.x,
            position.y + (lineHeight + spacing) * 2f,
            position.width,
            lineHeight
        );

        DrawVector2Clamped(row1, minProp, "Min");
        DrawVector2Clamped(row2, maxProp, "Max");

        var min = minProp.vector2Value;
        var max = maxProp.vector2Value;

        min.x = Mathf.Clamp01(min.x);
        min.y = Mathf.Clamp01(min.y);
        max.x = Mathf.Clamp01(max.x);
        max.y = Mathf.Clamp01(max.y);

        minProp.vector2Value = min;
        maxProp.vector2Value = max;

        var note = new GUIContent("Clamped to 0..1");
        EditorGUI.LabelField(row3, note);
        EditorGUI.EndProperty();
    }

    private void DrawVector2Clamped(Rect rect, SerializedProperty prop,
        string prefix)
    {
        var labelWidth = 35f;
        var fieldWidth = (rect.width - labelWidth) * 0.5f;

        var labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
        var xRect = new Rect(
            rect.x + labelWidth,
            rect.y,
            fieldWidth,
            rect.height
        );
        var yRect = new Rect(
            rect.x + labelWidth + fieldWidth,
            rect.y,
            fieldWidth,
            rect.height
        );

        EditorGUI.LabelField(labelRect, prefix);
        var value = prop.vector2Value;
        value.x = EditorGUI.Slider(xRect, value.x, 0f, 1f);
        value.y = EditorGUI.Slider(yRect, value.y, 0f, 1f);
        prop.vector2Value = value;
    }
}