using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(ExecuteEventHandler.ExecuteHandlerContext))]
public class ExecuteEventHandlerEditor : PropertyDrawer
{

    private int labelWidth = 80;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var eventContext = (ExecuteEventHandler.ExecuteHandlerContext)property.intValue;
        var targetObjectProperty = property.serializedObject.FindProperty("targetObject");

        var eventContextRect = new Rect(position.x, position.y, position.width, position.height);
        var staticObjectRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

        if (eventContext == ExecuteEventHandler.ExecuteHandlerContext.Static && targetObjectProperty != null)
        {
            // draw gameobject data editor
            EditorGUI.PropertyField(staticObjectRect, targetObjectProperty, GUIContent.none);

            eventContextRect.width = labelWidth;
        }

        EditorGUI.PropertyField(eventContextRect, property, GUIContent.none);


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
