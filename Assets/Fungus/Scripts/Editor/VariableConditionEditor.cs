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

        protected Dictionary<System.Type, SerializedProperty> propByVariableType;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            variableProp = serializedObject.FindProperty("variable");
            compareOperatorProp = serializedObject.FindProperty("compareOperator");

            // Get variable data props by name
            propByVariableType = new Dictionary<System.Type, SerializedProperty>() {
                { typeof(BooleanVariable), serializedObject.FindProperty("booleanData") },
                { typeof(IntegerVariable), serializedObject.FindProperty("integerData") },
                { typeof(FloatVariable), serializedObject.FindProperty("floatData") },
                { typeof(StringVariable), serializedObject.FindProperty("stringData") },
                { typeof(GameObjectVariable), serializedObject.FindProperty("gameObjectData") }
            };
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

            // Select Variable
            EditorGUILayout.PropertyField(variableProp);

            if (variableProp.objectReferenceValue == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Get selected variable
            Variable selectedVariable = variableProp.objectReferenceValue as Variable;
            System.Type variableType = selectedVariable.GetType();

            // Get operators for the variable
            CompareOperator[] compareOperators = VariableCondition.operatorsByVariableType[variableType];

            // Create operator list
            List<GUIContent> operatorsList = new List<GUIContent>();
            foreach (var compareOperator in compareOperators)
            {
                switch (compareOperator)
                {
                    case CompareOperator.Equals:
                        operatorsList.Add(new GUIContent("=="));
                        break;
                    case CompareOperator.NotEquals:
                        operatorsList.Add(new GUIContent("!="));
                        break;
                    case CompareOperator.LessThan:
                        operatorsList.Add(new GUIContent("<"));
                        break;
                    case CompareOperator.GreaterThan:
                        operatorsList.Add(new GUIContent(">"));
                        break;
                    case CompareOperator.LessThanOrEquals:
                        operatorsList.Add(new GUIContent("<="));
                        break;
                    case CompareOperator.GreaterThanOrEquals:
                        operatorsList.Add(new GUIContent(">="));
                        break;
                    default:
                        Debug.LogError("The " + compareOperator.ToString() + " operator has no matching GUIContent.");
                        break;
                }
            }

            // Get previously selected operator
            int selectedIndex = System.Array.IndexOf(compareOperators, t._CompareOperator);
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

            compareOperatorProp.enumValueIndex = (int)compareOperators[selectedIndex];

            EditorGUILayout.PropertyField(propByVariableType[variableType]);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
