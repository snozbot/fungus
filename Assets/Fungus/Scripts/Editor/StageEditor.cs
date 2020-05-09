// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
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

        public override void OnEnable()
        {
            base.OnEnable();

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
            string[] displayLabels = StringFormatter.FormatEnumNames(t.Display,"<None>");
            displayProp.enumValueIndex = EditorGUILayout.Popup("Display", (int)displayProp.enumValueIndex, displayLabels);

            string replaceLabel = "Portrait Stage";
            if (t.Display == StageDisplayType.Swap)
            {
                CommandEditor.ObjectField<Stage>(replacedStageProp, 
                                                 new GUIContent("Replace", "Character to swap with"), 
                                                 new GUIContent("<Default>"),
                                                 Stage.ActiveStages);
                replaceLabel = "With";
            }

            if (Stage.ActiveStages.Count > 0)
            {
                CommandEditor.ObjectField<Stage>(stageProp, 
                                                 new GUIContent(replaceLabel, "Stage to display the character portraits on"), 
                                                 new GUIContent("<Default>"),
                                                 Stage.ActiveStages);
            }

            bool showOptionalFields = true;
            Stage s = t._Stage;
            // Only show optional portrait fields once required fields have been filled...
            if (t._Stage != null)                // Character is selected
            {
                if (t._Stage == null)        // If no default specified, try to get any portrait stage in the scene
                {
                    s = GameObject.FindObjectOfType<Stage>();
                }
                if (s == null)
                {
                    EditorGUILayout.HelpBox("No portrait stage has been set.", MessageType.Error);
                    showOptionalFields = false; 
                }
            }
            if (t.Display != StageDisplayType.None && showOptionalFields) 
            {
                EditorGUILayout.PropertyField(useDefaultSettingsProp);
                if (!t.UseDefaultSettings)
                {
                    EditorGUILayout.PropertyField(fadeDurationProp);
                }
                EditorGUILayout.PropertyField(waitUntilFinishedProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}