// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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

        public override void OnEnable()
        {
            base.OnEnable();

            textProp = serializedObject.FindProperty("text");
            descriptionProp = serializedObject.FindProperty("description");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
            hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
            interactableProp = serializedObject.FindProperty("interactable");
            setMenuDialogProp = serializedObject.FindProperty("setMenuDialog");
            hideThisOptionProp = serializedObject.FindProperty("hideThisOption");
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

            EditorGUILayout.BeginHorizontal();
            BlockEditor.BlockField(targetBlockProp,
                                   new GUIContent("Target Block", "Block to call when option is selected"), 
                                   new GUIContent("<None>"), 
                                   flowchart);
            const int popupWidth = 17;
            if(targetBlockProp.objectReferenceValue == null && GUILayout.Button("+",GUILayout.MaxWidth(popupWidth)))
            {
                var fw = EditorWindow.GetWindow<FlowchartWindow>();
                var t = (Menu)target;
                var activeFlowchart = t.GetFlowchart();
                var newBlock = fw.CreateBlockSuppressSelect(activeFlowchart, t.ParentBlock._NodeRect.position - Vector2.down * 60);
                targetBlockProp.objectReferenceValue = newBlock;
                activeFlowchart.SelectedBlock = t.ParentBlock;
            }
            EditorGUILayout.EndHorizontal();



            EditorGUILayout.PropertyField(hideIfVisitedProp);
            EditorGUILayout.PropertyField(interactableProp);
            EditorGUILayout.PropertyField(setMenuDialogProp);
            EditorGUILayout.PropertyField(hideThisOptionProp);
            
            serializedObject.ApplyModifiedProperties();
        }
    }    
}
