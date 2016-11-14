// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Call))]
    public class CallEditor : CommandEditor 
    {
        protected SerializedProperty targetFlowchartProp;
        protected SerializedProperty targetBlockProp;
        protected SerializedProperty startLabelProp;
        protected SerializedProperty startIndexProp;
        protected SerializedProperty callModeProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            targetFlowchartProp = serializedObject.FindProperty("targetFlowchart");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
            startLabelProp = serializedObject.FindProperty("startLabel");
            startIndexProp = serializedObject.FindProperty("startIndex");
            callModeProp = serializedObject.FindProperty("callMode");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            Call t = target as Call;

            Flowchart flowchart = null;
            if (targetFlowchartProp.objectReferenceValue == null)
            {
                flowchart = (Flowchart)t.GetFlowchart();
            }
            else
            {
                flowchart = targetFlowchartProp.objectReferenceValue as Flowchart;
            }

            EditorGUILayout.PropertyField(targetFlowchartProp);

            if (flowchart != null)
            {
                BlockEditor.BlockField(targetBlockProp,
                                       new GUIContent("Target Block", "Block to call"), 
                                       new GUIContent("<None>"), 
                                       flowchart);

                EditorGUILayout.PropertyField(startLabelProp);

                EditorGUILayout.PropertyField(startIndexProp);
            }

            EditorGUILayout.PropertyField(callModeProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
