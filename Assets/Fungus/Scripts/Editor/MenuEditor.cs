// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet changed by ducksonthewater, 2019-01-10 - www.ducks-on-the-water.com

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Menu))]
    public class MenuEditor : CommandEditor 
    {
        protected SerializedProperty textProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty targetBlockProp;
        protected SerializedProperty hideIfVisitedProp;
        protected SerializedProperty interactableProp;
        protected SerializedProperty setMenuDialogProp;
        protected SerializedProperty hideThisOptionProp;
        protected SerializedProperty menuImageProp;  // added by ducksonthewater, 2019-01-10

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            textProp = serializedObject.FindProperty("text");
            descriptionProp = serializedObject.FindProperty("description");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
            hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
            interactableProp = serializedObject.FindProperty("interactable");
            setMenuDialogProp = serializedObject.FindProperty("setMenuDialog");
            hideThisOptionProp = serializedObject.FindProperty("hideThisOption");
            menuImageProp = serializedObject.FindProperty("menuImage");    // added by ducksonthewater, 2019-01-10
        }
        
        public override void DrawCommandGUI()
        {
            var flowchart = FlowchartWindow.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(textProp);

            EditorGUILayout.PropertyField(descriptionProp);
            
            BlockEditor.BlockField(targetBlockProp,
                                   new GUIContent("Target Block", "Block to call when option is selected"), 
                                   new GUIContent("<None>"), 
                                   flowchart);
            
            EditorGUILayout.PropertyField(hideIfVisitedProp);
            EditorGUILayout.PropertyField(interactableProp);
            EditorGUILayout.PropertyField(setMenuDialogProp);
            EditorGUILayout.PropertyField(hideThisOptionProp);
            EditorGUILayout.PropertyField(menuImageProp);   // added by ducksonthewater, 2019-01-10

            serializedObject.ApplyModifiedProperties();
        }
    }    
}
