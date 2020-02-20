// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(VariableCondition), true)]
    public class VariableConditionEditor : CommandEditor
    {
        public static readonly GUIContent None = new GUIContent("<None>");

        public static readonly GUIContent[] emptyList = new GUIContent[]
        {
            None,
        };

        static readonly GUIContent[] compareListAll = new GUIContent[]
        {
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.Equals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.NotEquals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.LessThan)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.GreaterThan)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.LessThanOrEquals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.GreaterThanOrEquals)),
        };

        static readonly GUIContent[] compareListEqualOnly = new GUIContent[]
        {
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.Equals)),
            new GUIContent(VariableUtil.GetCompareOperatorDescription(CompareOperator.NotEquals)),
        };

        // protected SerializedProperty compareOperatorProp;
        // protected SerializedProperty anyVarProp;
        
         protected SerializedProperty conditions;

        protected Dictionary<System.Type, SerializedProperty> propByVariableType;

        public override void OnEnable()
        {
            base.OnEnable();

            // compareOperatorProp = serializedObject.FindProperty("compareOperator");
            // anyVarProp = serializedObject.FindProperty("anyVar");

            conditions = serializedObject.FindProperty("conditions");

        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("anyOrAllConditions"));            

            conditions.arraySize = EditorGUILayout.IntField("Size", conditions.arraySize);

            for (int i = 0; i < conditions.arraySize; i++)
            {

                VariableCondition t = target as VariableCondition;

                var flowchart = (Flowchart)t.GetFlowchart();
                if (flowchart == null)
                {
                    return;
                }

                // EditorGUILayout.PropertyField(anyVarProp, true);
                var conditionAnyVar = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("anyVar");
                var conditionCompare = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("compareOperator");
                
                EditorGUILayout.PropertyField(conditionAnyVar,new GUIContent("Variable"),true);

                // Get selected variable
                Variable selectedVariable = conditionAnyVar.FindPropertyRelative("variable").objectReferenceValue as Variable;
                GUIContent[] operatorsList = emptyList;
                if (selectedVariable != null)
                {
                    operatorsList = selectedVariable.IsComparisonSupported() ? compareListAll : compareListEqualOnly;
                }
                
                // Get previously selected operator
                int selectedIndex = (int)t.Conditions[i].CompareOperator;
                if (selectedIndex < 0)
                {
                    // Default to first index if the operator is not found in the available operators list
                    // This can occur when changing between variable types
                    selectedIndex = 0;
                }

                selectedIndex = EditorGUILayout.Popup(
                    new GUIContent("Compare", "The comparison operator to use when comparing values"),
                    selectedIndex,
                    operatorsList);

                if (selectedVariable != null)
                {
                    conditionCompare.enumValueIndex = selectedIndex;
                }


                
            }

           
            serializedObject.ApplyModifiedProperties();
        }
    }
}
