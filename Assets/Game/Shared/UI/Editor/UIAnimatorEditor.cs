using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UIAnimatorAnimationStyle))]
public class UIAnimatorAnimationStyleDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var animFieldProp = property.FindPropertyRelative("animationField");
        var fieldType = (UIAnimationFieldType)animFieldProp.enumValueIndex;

        // Base fields: animationField, curve, duration, startDelay, loopType = 5
        var lineCount = 5;

        // Type-specific fields
        lineCount += fieldType switch {
            UIAnimationFieldType.Position => 2, // fromPosition, toPosition
            UIAnimationFieldType.Rotation => 2, // fromRotation, toRotation
            UIAnimationFieldType.Scale => 2, // fromScale, toScale
            UIAnimationFieldType.Color => 1, // colorGradient
            var _ => 0
        };

        return lineCount * EditorGUIUtility.singleLineHeight +
               (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        var animFieldProp = property.FindPropertyRelative("animationField");
        var curveProp = property.FindPropertyRelative("curve");
        var durationProp = property.FindPropertyRelative("duration");
        var startDelayProp = property.FindPropertyRelative("startDelay");
        var loopTypeProp = property.FindPropertyRelative("loopType");

        var fieldType = (UIAnimationFieldType)animFieldProp.enumValueIndex;

        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        // Draw base fields
        EditorGUI.PropertyField(rect, animFieldProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.PropertyField(rect, curveProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.PropertyField(rect, durationProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.PropertyField(rect, startDelayProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.PropertyField(rect, loopTypeProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw type-specific fields
        switch (fieldType) {
            case UIAnimationFieldType.Position:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("fromPosition"));
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("toPosition"));
                break;

            case UIAnimationFieldType.Rotation:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("fromRotation"));
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("toRotation"));
                break;

            case UIAnimationFieldType.Scale:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("fromScale"));
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("toScale"));
                break;

            case UIAnimationFieldType.Color:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("colorGradient"));
                break;
        }

        EditorGUI.EndProperty();
    }
}