// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{

    [CustomEditor (typeof(SetVariable))]
    public class SetVariableEditor : CommandEditor
    {
        protected SerializedProperty anyVarProp;
        protected SerializedProperty setOperatorProp;
        
        public override void OnEnable()
        {
            base.OnEnable();

            anyVarProp = serializedObject.FindProperty("anyVar");
            setOperatorProp = serializedObject.FindProperty("setOperator");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            SetVariable t = target as SetVariable;

            var flowchart = (Flowchart)t.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            // Select Variable
            EditorGUILayout.PropertyField(anyVarProp, true);

            //fetching every draw to ensure we don't have stale data based on types that have changed by user selection,
            //  without us noticing.

            // Get selected variable
            Variable selectedVariable = anyVarProp.FindPropertyRelative("variable").objectReferenceValue as Variable;
            List<GUIContent> operatorsList = new List<GUIContent>();
            if (selectedVariable != null)
            {
                if(selectedVariable.IsArithmeticSupported(SetOperator.Assign))
                    operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Assign)));
                if (selectedVariable.IsArithmeticSupported(SetOperator.Negate))
                    operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Negate)));
                if (selectedVariable.IsArithmeticSupported(SetOperator.Add))
                    operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Add)));
                if (selectedVariable.IsArithmeticSupported(SetOperator.Subtract))
                    operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Subtract)));
                if (selectedVariable.IsArithmeticSupported(SetOperator.Multiply))
                    operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Multiply)));
                if (selectedVariable.IsArithmeticSupported(SetOperator.Divide))
                    operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Divide)));
            }
            else
            {
                operatorsList.Add(VariableConditionEditor.None);
            }

            // Get previously selected operator
            int selectedIndex = (int) t._SetOperator;
            if (selectedIndex < 0)
            {
                // Default to first index if the operator is not found in the available operators list
                // This can occur when changing between variable types
                selectedIndex = 0;
            }

            // Get next selected operator
            selectedIndex = EditorGUILayout.Popup(new GUIContent("Operation", "Arithmetic operator to use"), selectedIndex, operatorsList.ToArray());

            if (selectedVariable != null)
            {
                setOperatorProp.enumValueIndex = selectedIndex;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
