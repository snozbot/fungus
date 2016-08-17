/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;


namespace Fungus
{
    
    [CustomEditor (typeof(ControlStage))]
    public class StageEditor : CommandEditor
    {
        protected SerializedProperty displayProp;
        protected SerializedProperty stageProp;
        protected SerializedProperty replacedStageProp;
        protected SerializedProperty useDefaultSettingsProp;
        protected SerializedProperty fadeDurationProp;
        protected SerializedProperty waitUntilFinishedProp;
        
        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            displayProp = serializedObject.FindProperty("display");
            stageProp = serializedObject.FindProperty("stage");
            replacedStageProp = serializedObject.FindProperty("replacedStage");
            useDefaultSettingsProp = serializedObject.FindProperty("useDefaultSettings");
            fadeDurationProp = serializedObject.FindProperty("fadeDuration");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
        }
        
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();
            
            ControlStage t = target as ControlStage;

            // Format Enum names
            string[] displayLabels = StringFormatter.FormatEnumNames(t.display,"<None>");
            displayProp.enumValueIndex = EditorGUILayout.Popup("Display", (int)displayProp.enumValueIndex, displayLabels);

            string replaceLabel = "Portrait Stage";
            if (t.display == StageDisplayType.Swap)
            {
                CommandEditor.ObjectField<Stage>(replacedStageProp, 
                                                 new GUIContent("Replace", "Character to swap with"), 
                                                 new GUIContent("<Default>"),
                                                 Stage.activeStages);
                replaceLabel = "With";
            }

            if (Stage.activeStages.Count > 0)
            {
                CommandEditor.ObjectField<Stage>(stageProp, 
                                                 new GUIContent(replaceLabel, "Stage to display the character portraits on"), 
                                                 new GUIContent("<Default>"),
                                                 Stage.activeStages);
            }

            bool showOptionalFields = true;
            Stage s = t.stage;
            // Only show optional portrait fields once required fields have been filled...
            if (t.stage != null)                // Character is selected
            {
                if (t.stage == null)        // If no default specified, try to get any portrait stage in the scene
                {
                    s = GameObject.FindObjectOfType<Stage>();
                }
                if (s == null)
                {
                    EditorGUILayout.HelpBox("No portrait stage has been set.", MessageType.Error);
                    showOptionalFields = false; 
                }
            }
            if (t.display != StageDisplayType.None && showOptionalFields) 
            {
                EditorGUILayout.PropertyField(useDefaultSettingsProp);
                if (!t.useDefaultSettings)
                {
                    EditorGUILayout.PropertyField(fadeDurationProp);
                }
                EditorGUILayout.PropertyField(waitUntilFinishedProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}