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
        protected SerializedProperty variableProp;
        protected SerializedProperty compareOperatorProp;
        protected SerializedProperty booleanDataProp;
        protected SerializedProperty integerDataProp;
        protected SerializedProperty floatDataProp;
        protected SerializedProperty stringDataProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            variableProp = serializedObject.FindProperty("variable");
            compareOperatorProp = serializedObject.FindProperty("compareOperator");
            booleanDataProp = serializedObject.FindProperty("booleanData");
            integerDataProp = serializedObject.FindProperty("integerData");
            floatDataProp = serializedObject.FindProperty("floatData");
            stringDataProp = serializedObject.FindProperty("stringData");
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

            EditorGUILayout.PropertyField(variableProp);

            if (variableProp.objectReferenceValue == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            Variable selectedVariable = variableProp.objectReferenceValue as Variable;
            System.Type variableType = selectedVariable.GetType();

            List<GUIContent> operatorList = new List<GUIContent>();
            operatorList.Add(new GUIContent("=="));
            operatorList.Add(new GUIContent("!="));
            if (variableType == typeof(IntegerVariable) ||
                variableType == typeof(FloatVariable))
            {
                operatorList.Add(new GUIContent("<"));
                operatorList.Add(new GUIContent(">"));
                operatorList.Add(new GUIContent("<="));
                operatorList.Add(new GUIContent(">="));
            }

            compareOperatorProp.enumValueIndex = EditorGUILayout.Popup(new GUIContent("Compare", "The comparison operator to use when comparing values"), 
                                                                       compareOperatorProp.enumValueIndex, 
                                                                       operatorList.ToArray());

            if (variableType == typeof(BooleanVariable))
            {
                EditorGUILayout.PropertyField(booleanDataProp);
            }
            else if (variableType == typeof(IntegerVariable))
            {
                EditorGUILayout.PropertyField(integerDataProp);
            }
            else if (variableType == typeof(FloatVariable))
            {
                EditorGUILayout.PropertyField(floatDataProp);
            }
            else if (variableType == typeof(StringVariable))
            {
                EditorGUILayout.PropertyField(stringDataProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
