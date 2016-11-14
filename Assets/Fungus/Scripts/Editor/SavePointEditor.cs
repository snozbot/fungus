// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using Fungus;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(SavePoint))]
    public class SavePointEditor : CommandEditor
    {
        protected SerializedProperty resumeBlockProp;
        protected SerializedProperty saveNowProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            resumeBlockProp = serializedObject.FindProperty("resumeBlock");
            saveNowProp = serializedObject.FindProperty("saveNow");
        }

        public override void DrawCommandGUI()
        {
            var flowchart = FlowchartWindow.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            serializedObject.Update();

            BlockEditor.BlockField(resumeBlockProp,
                                   new GUIContent("Resume Block", "Block to call when save data is loaded again"), 
                                   new GUIContent("<None>"), 
                                   flowchart);
            EditorGUILayout.PropertyField(saveNowProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
