using UnityEngine;
using UnityEditor;
using System.Linq;

// Source: https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/

[CustomPropertyDrawer(typeof(ProgressInterfaceAttribute))]
public class ProgressInterfaceDrawer : PropertyDrawer
{
    // Overrides GUI drawing for the attribute.
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if this is reference type property.
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // Get attribute parameters.
            var requiredAttribute = this.attribute as ProgressInterfaceAttribute;

            // Begin drawing property field.
            EditorGUI.BeginProperty(position, label, property);

            // Draw property field.
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);

            // Finish drawing property field.
            EditorGUI.EndProperty();
        }

        else
        {
            // If field is not reference, show error message.
            // Save previous color and change GUI to red.

            var previousColor = GUI.color;
            GUI.color = Color.red;

            // Display label with error message.
            EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));

            GUI.color = previousColor;
        }
    }
}
