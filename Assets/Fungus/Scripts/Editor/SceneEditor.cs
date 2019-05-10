// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-01-14 - www.ducks-on-the-water.com

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SceneLoadAdditive))]
    public class SceneEditor : CommandEditor 
    {
        protected SerializedProperty sceneNameProp;
        protected SerializedProperty showSceneWhenLoadedProp;
        protected SerializedProperty targetFlowchartProp;
        protected SerializedProperty targetBlockProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            sceneNameProp = serializedObject.FindProperty("sceneName");
            showSceneWhenLoadedProp = serializedObject.FindProperty("showSceneWhenLoaded");
            targetFlowchartProp = serializedObject.FindProperty("targetFlowchart");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            SceneLoadAdditive t = target as SceneLoadAdditive;

            Flowchart flowchart = null;
            if (targetFlowchartProp.objectReferenceValue == null)
            {
                flowchart = (Flowchart)t.GetFlowchart();
            }
            else
            {
                flowchart = targetFlowchartProp.objectReferenceValue as Flowchart;
            }

            EditorGUILayout.PropertyField(sceneNameProp);

            EditorGUILayout.PropertyField(showSceneWhenLoadedProp);

            EditorGUILayout.PropertyField(targetFlowchartProp);

            if (flowchart != null)
            {
                BlockEditor.BlockField(targetBlockProp,
                                       new GUIContent("Target Block", "Block to call"), 
                                       new GUIContent("<None>"), 
                                       flowchart);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
