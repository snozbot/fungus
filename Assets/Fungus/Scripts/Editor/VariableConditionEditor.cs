// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(VariableCondition), true)]
    public class VariableConditionEditor : CommandEditor
    {
        public static readonly List<GUIContent> emptyList = new List<GUIContent>()
        {
            new GUIContent("<None>"),
        };

        static readonly List<GUIContent> compareListAll = new List<GUIContent>()
        {
            new GUIContent("=="),
            new GUIContent("!="),
            new GUIContent("<"),
            new GUIContent(">"),
            new GUIContent("<="),
            new GUIContent(">="),
        };

        static readonly List<GUIContent> compareListEqualOnly = new List<GUIContent>()
        {
            new GUIContent("=="),
            new GUIContent("!="),
        };

        protected SerializedProperty compareOperatorProp;
        protected SerializedProperty anyVarProp;

        protected Dictionary<System.Type, SerializedProperty> propByVariableType;

        public override void OnEnable()
        {
            base.OnEnable();

            compareOperatorProp = serializedObject.FindProperty("compareOperator");
            anyVarProp = serializedObject.FindProperty("anyVar");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            VariableCondition t = target as VariableCondition;

            var flowchart = (Flowchart)t.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            EditorGUILayout.PropertyField(anyVarProp, true);

            // Get selected variable
            Variable selectedVariable = anyVarProp.FindPropertyRelative("variable").objectReferenceValue as Variable;
            List<GUIContent> operatorsList = emptyList;
            if (selectedVariable != null)
            {
                operatorsList = selectedVariable.IsComparisonSupported() ? compareListAll : compareListEqualOnly;
            }
            
            // Get previously selected operator
            int selectedIndex = (int)t._CompareOperator;
            if (selectedIndex < 0)
            {
                // Default to first index if the operator is not found in the available operators list
                // This can occur when changing between variable types
                selectedIndex = 0;
            }

            selectedIndex = EditorGUILayout.Popup(
                new GUIContent("Compare", "The comparison operator to use when comparing values"),
                selectedIndex,
                operatorsList.ToArray());

            if (selectedVariable != null)
            {
                compareOperatorProp.enumValueIndex = selectedIndex;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
