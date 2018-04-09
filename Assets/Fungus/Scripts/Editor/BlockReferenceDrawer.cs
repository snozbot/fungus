using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Custom drawer for the BlockReference, allows for more easily selecting a target block in external c#
    /// scripts.
    /// </summary>
    [CustomPropertyDrawer(typeof(Fungus.BlockReference))]
    public class BlockReferenceDrawer : PropertyDrawer
    {
        public Fungus.Flowchart lastFlowchart;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var l = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, l);
            position.height = EditorGUIUtility.singleLineHeight;
            var block = property.FindPropertyRelative("block");

            Fungus.Block b = block.objectReferenceValue as Fungus.Block;

            if (block.objectReferenceValue != null && lastFlowchart == null)
            {
                if (b != null)
                {
                    lastFlowchart = b.GetFlowchart();
                }
            }

            lastFlowchart = EditorGUI.ObjectField(position, lastFlowchart, typeof(Fungus.Flowchart), true) as Fungus.Flowchart;
            position.y += EditorGUIUtility.singleLineHeight;
            if (lastFlowchart != null)
                b = Fungus.EditorUtils.BlockEditor.BlockField(position, new GUIContent("None"), lastFlowchart, b);
            else
                EditorGUI.PrefixLabel(position, new GUIContent("Flowchart Required"));

            block.objectReferenceValue = b;

            block.serializedObject.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}