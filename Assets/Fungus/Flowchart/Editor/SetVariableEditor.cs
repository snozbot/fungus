/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

    [CustomEditor (typeof(SetVariable))]
    public class SetVariableEditor : CommandEditor 
    {
        protected SerializedProperty variableProp;
        protected SerializedProperty setOperatorProp;
        protected SerializedProperty booleanDataProp;
        protected SerializedProperty integerDataProp;
        protected SerializedProperty floatDataProp;
        protected SerializedProperty stringDataProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            variableProp = serializedObject.FindProperty("variable");
            setOperatorProp = serializedObject.FindProperty("setOperator");
            booleanDataProp = serializedObject.FindProperty("booleanData");
            integerDataProp = serializedObject.FindProperty("integerData");
            floatDataProp = serializedObject.FindProperty("floatData");
            stringDataProp = serializedObject.FindProperty("stringData");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            SetVariable t = target as SetVariable;

            Flowchart flowchart = t.GetFlowchart();
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

            List<GUIContent> operatorsList = new List<GUIContent>();
            operatorsList.Add(new GUIContent("="));
            if (variableType == typeof(BooleanVariable))
            {
                operatorsList.Add(new GUIContent("=!"));
            }
            else if (variableType == typeof(IntegerVariable) ||
                     variableType == typeof(FloatVariable))
            {
                operatorsList.Add(new GUIContent("+="));
                operatorsList.Add(new GUIContent("-="));
                operatorsList.Add(new GUIContent("*="));
                operatorsList.Add(new GUIContent("/="));
            }
            
            int selectedIndex = 0;
            switch (t.setOperator)
            {
                default:
                case SetVariable.SetOperator.Assign:
                    selectedIndex = 0;
                    break;
                case SetVariable.SetOperator.Negate:
                    selectedIndex = 1;
                    break;
                case SetVariable.SetOperator.Add:
                    selectedIndex = 1;
                    break;
                case SetVariable.SetOperator.Subtract:
                    selectedIndex = 2;
                    break;
                case SetVariable.SetOperator.Multiply:
                    selectedIndex = 3;
                    break;
                case SetVariable.SetOperator.Divide:
                    selectedIndex = 4;
                    break;
            }

            selectedIndex = EditorGUILayout.Popup(new GUIContent("Operation", "Arithmetic operator to use"), selectedIndex, operatorsList.ToArray());
            
            SetVariable.SetOperator setOperator = SetVariable.SetOperator.Assign;
            if (variableType == typeof(BooleanVariable) || 
                variableType == typeof(StringVariable))
            {
                switch (selectedIndex)
                {
                default:
                case 0:
                    setOperator = SetVariable.SetOperator.Assign;
                    break;
                case 1:
                    setOperator = SetVariable.SetOperator.Negate;
                    break;
                }
            } 
            else if (variableType == typeof(IntegerVariable) || 
                     variableType == typeof(FloatVariable))
            {
                switch (selectedIndex)
                {
                default:
                case 0:
                    setOperator = SetVariable.SetOperator.Assign;
                    break;
                case 1:
                    setOperator = SetVariable.SetOperator.Add;
                    break;
                case 2:
                    setOperator = SetVariable.SetOperator.Subtract;
                    break;
                case 3:
                    setOperator = SetVariable.SetOperator.Multiply;
                    break;
                case 4:
                    setOperator = SetVariable.SetOperator.Divide;
                    break;
                }
            }

            setOperatorProp.enumValueIndex = (int)setOperator;

            if (variableType == typeof(BooleanVariable))
            {
                EditorGUILayout.PropertyField(booleanDataProp, new GUIContent("Boolean"));
            }
            else if (variableType == typeof(IntegerVariable))
            {
                EditorGUILayout.PropertyField(integerDataProp, new GUIContent("Integer"));
            }
            else if (variableType == typeof(FloatVariable))
            {
                EditorGUILayout.PropertyField(floatDataProp, new GUIContent("Float"));
            }
            else if (variableType == typeof(StringVariable))
            {
                EditorGUILayout.PropertyField(stringDataProp, new GUIContent("String"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

}
