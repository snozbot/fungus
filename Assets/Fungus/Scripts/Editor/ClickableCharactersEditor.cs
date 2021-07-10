// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(ClickableCharacters))]
    public class ClickableCharactersEditor : CommandEditor
    {
        protected SerializedProperty stateProp;
        protected SerializedProperty characterProp;
        protected SerializedProperty flowchartProp;
        protected SerializedProperty targetBlockProp;

        public override void OnEnable()
        {
            base.OnEnable();

            stateProp = serializedObject.FindProperty("activeState");
            characterProp = serializedObject.FindProperty("character");
            flowchartProp = serializedObject.FindProperty("flowchart");
            targetBlockProp = serializedObject.FindProperty("executeBlock");
        }
        
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();
            
            ClickableCharacters t = target as ClickableCharacters;

            string characterLabel = "Character";

            EditorGUILayout.PropertyField(stateProp, new GUIContent("Active State", "Enable or Disable"));
            
            CommandEditor.ObjectField<Character>(characterProp, 
                                                 new GUIContent(characterLabel, "Character to display"), 
                                                 new GUIContent("<None>"),
                                                 Character.ActiveCharacters);

            Flowchart flowchart = null;
            if (flowchartProp.objectReferenceValue == null)
            {
                flowchart = (Flowchart)t.GetFlowchart();
            }
            else
            {
                flowchart = flowchartProp.objectReferenceValue as Flowchart;
            }

            EditorGUILayout.PropertyField(flowchartProp);

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