using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(KeyEventProgressData))]
public class KeyEventProgressDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty showInkFileName = property.FindPropertyRelative("ShowInkFileName");
        SerializedProperty showAnimationState = property.FindPropertyRelative("ShowAnimationState");
        SerializedProperty showFacingDir = property.FindPropertyRelative("ShowFacingDir");
        SerializedProperty showIsVisible = property.FindPropertyRelative("ShowIsVisible");
        SerializedProperty showPosition = property.FindPropertyRelative("ShowPosition");

        SerializedProperty keyEvent = property.FindPropertyRelative("KeyEvent");
        SerializedProperty inkFileName = property.FindPropertyRelative("InkFileName");
        SerializedProperty animationState = property.FindPropertyRelative("AnimationState");
        SerializedProperty facingDir = property.FindPropertyRelative("FacingDir");
        SerializedProperty isVisible = property.FindPropertyRelative("IsVisible");
        SerializedProperty targetPos = property.FindPropertyRelative("Position");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float startX = 30;
        float toggleWidth = 20;
        float labelOffset = 17;
        float labelWidth = 100;
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float offSet = lineHeight + 3;

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var r = new Rect(startX + 85, position.y, position.width, lineHeight);

        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var r1 = new Rect(startX, position.y + offSet * 0, toggleWidth, lineHeight);
        var r2 = new Rect(startX, position.y + offSet * 1, toggleWidth, lineHeight);
        var r3 = new Rect(startX, position.y + offSet * 2, toggleWidth, lineHeight);
        var r4 = new Rect(startX, position.y + offSet * 3, toggleWidth, lineHeight);
        var r5 = new Rect(startX, position.y + offSet * 4, toggleWidth, lineHeight);

        EditorGUI.PropertyField(r, keyEvent, GUIContent.none);

        showInkFileName.boolValue = EditorGUI.Toggle(r1, showInkFileName.boolValue);
        Rect labelRect = new Rect(r1.x + labelOffset, r1.y, labelWidth, r1.height);
        EditorGUI.LabelField(labelRect, "Change Ink");
        if (showInkFileName.boolValue) {
            r1.x += 100;
            r1.width = 200;
            inkFileName.stringValue = EditorGUI.TextField(r1, inkFileName.stringValue);
        }

        showAnimationState.boolValue = EditorGUI.Toggle(r2, showAnimationState.boolValue);
        labelRect = new Rect(r2.x + labelOffset, r2.y, labelWidth, r2.height);
        EditorGUI.LabelField(labelRect, "Change Anim");
        if (showAnimationState.boolValue) {
            r2.x += 100;
            r2.width = 200;
            animationState.stringValue = EditorGUI.TextField(r2, animationState.stringValue);
        }

        showFacingDir.boolValue = EditorGUI.Toggle(r3, showFacingDir.boolValue);
        labelRect = new Rect(r3.x + labelOffset, r3.y, labelWidth, r3.height);
        EditorGUI.LabelField(labelRect, "Change Facing Dir");
        if (showFacingDir.boolValue) {
            r3.x += 100;
            r3.width = 200;
            facingDir.stringValue = EditorGUI.TextField(r3, facingDir.stringValue);
        }

        showIsVisible.boolValue = EditorGUI.Toggle(r5, showIsVisible.boolValue);
        labelRect = new Rect(r5.x + labelOffset, r5.y, labelWidth, r2.height);
        EditorGUI.LabelField(labelRect, "Change Visibility");
        if (showIsVisible.boolValue) {
            r5.x += 120;
            r5.width = toggleWidth;
            isVisible.boolValue = EditorGUI.Toggle(r5, isVisible.boolValue);
        }

        showPosition.boolValue = EditorGUI.Toggle(r4, showPosition.boolValue);
        labelRect = new Rect(r4.x + labelOffset, r4.y, labelWidth, r2.height);
        EditorGUI.LabelField(labelRect, "Change Position");
        if (showPosition.boolValue) {
            r4.x += 120;
            r4.width = 150;
            targetPos.vector2Value = EditorGUI.Vector2Field(r4, "", targetPos.vector2Value);
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Return the total height required for drawing all properties
        return EditorGUIUtility.singleLineHeight * 7;
    }
}
