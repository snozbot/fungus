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
#if UNITY_LOCALIZATION
        protected SerializedProperty localizedTextProp;
#endif
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
#if UNITY_LOCALIZATION
            localizedTextProp = serializedObject.FindProperty("localizedText");
#endif
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
            
            Menu t = target as Menu;

#if !UNITY_LOCALIZATION
            EditorGUILayout.PropertyField(textProp);
#else
            string textTitle = "Text";
            if (!t.GetLocalizedStringComponent().IsEmpty)
                textTitle += " (IGNORED FOR LOCALIZED TEXT)";
            EditorGUILayout.PropertyField(textProp, new GUIContent(textTitle, "Text to display on the menu button. Ignored if Localized Text is not empty."));
            EditorGUILayout.PropertyField(localizedTextProp);
#endif

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
