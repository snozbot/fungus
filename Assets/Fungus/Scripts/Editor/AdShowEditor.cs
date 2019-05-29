// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-05-29 - www.ducks-on-the-water.com

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(AdShow))]
    public class AdShowEditor : CommandEditor
    {
        protected SerializedProperty placementIdProp;
        protected SerializedProperty targetBlockSuccessProp;
        protected SerializedProperty targetBlockCancelProp;
        protected SerializedProperty targetBlockErrorProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            placementIdProp = serializedObject.FindProperty("placementId");
            targetBlockSuccessProp = serializedObject.FindProperty("targetBlockSuccess");
            targetBlockCancelProp = serializedObject.FindProperty("targetBlockCancel");
            targetBlockErrorProp = serializedObject.FindProperty("targetBlockError");
        }

        public override void DrawCommandGUI()
        {
            var flowchart = FlowchartWindow.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(placementIdProp, new GUIContent("Placement ID","e.g. video - from Unity Ads Backend."));

            BlockEditor.BlockField(targetBlockSuccessProp,
                                   new GUIContent("Target Block Success", "Block to call when Ad was successfully shown."),
                                   new GUIContent("<None>"),
                                   flowchart);

            BlockEditor.BlockField(targetBlockCancelProp,
                                   new GUIContent("Target Block Cancel", "Block to call when Ad was cancelled by user."),
                                   new GUIContent("<None>"),
                                   flowchart);

            BlockEditor.BlockField(targetBlockErrorProp,
                                   new GUIContent("Target Block Error", "Block to call when an error occured."),
                                   new GUIContent("<None>"),
                                   flowchart);

            serializedObject.ApplyModifiedProperties();
        }
    }
}