// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Handles custom drawing for ConditionExperssions within the VariableCondition and inherited commands.
    /// 
    /// TODO; refactor to allow a propertydrawer on ConditionExperssion and potentially list as reorderable
    /// </summary>
    [CustomEditor(typeof(VariableCondition), true)]
    public class VariableConditionEditor : CommandEditor
    {
        public static readonly GUIContent None = new GUIContent("<None>");

        public static readonly GUIContent[] emptyList = new GUIContent[]
        {
            None,
        };

        private static readonly GUIContent[] compareListAll = new GUIContent[]
        {
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.Equals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.NotEquals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.LessThan)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.GreaterThan)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.LessThanOrEquals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.GreaterThanOrEquals)),
        };

        private static readonly GUIContent[] compareListEqualOnly = new GUIContent[]
        {
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.Equals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.NotEquals)),
        };

        protected SerializedProperty conditions;

        public override void OnEnable()
        {
            base.OnEnable();

            conditions = serializedObject.FindProperty("conditions");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("anyOrAllConditions"));

            conditions.arraySize = EditorGUILayout.IntField("Size", conditions.arraySize);
            GUILayout.Label("Conditions", EditorStyles.boldLabel);

            VariableCondition t = target as VariableCondition;

            var flowchart = (Flowchart)t.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < conditions.arraySize; i++)
            {
                var conditionAnyVar = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("anyVar");
                var conditionCompare = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("compareOperator");

                EditorGUILayout.PropertyField(conditionAnyVar, new GUIContent("Variable"), true);

                // Get selected variable
                Variable selectedVariable = conditionAnyVar.FindPropertyRelative("variable").objectReferenceValue as Variable;

                if (selectedVariable == null)
                    continue;

                GUIContent[] operatorsList = emptyList;
                operatorsList = selectedVariable.IsComparisonSupported() ? compareListAll : compareListEqualOnly;

                // Get previously selected operator
                int selectedIndex = conditionCompare.enumValueIndex;
                if (selectedIndex < 0 || selectedIndex >= operatorsList.Length)
                {
                    // Default to first index if the operator is not found in the available operators list
                    // This can occur when changing between variable types
                    selectedIndex = 0;
                }

                selectedIndex = EditorGUILayout.Popup(
                    new GUIContent("Compare", "The comparison operator to use when comparing values"),
                    selectedIndex,
                    operatorsList);

                conditionCompare.enumValueIndex = selectedIndex;

                EditorGUILayout.Separator();
            }
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}