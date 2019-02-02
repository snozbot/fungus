// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{

    [CustomEditor (typeof(SetVariable))]
    public class SetVariableEditor : CommandEditor
    {
        static readonly List<GUIContent> operatorsListAll = new List<GUIContent>()
        {
            new GUIContent("="),
            new GUIContent("=!"),
            new GUIContent("+="),
            new GUIContent("-="),
            new GUIContent("*="),
            new GUIContent("\\="),
        };

        static readonly List<GUIContent> operatorsListAssignOnly = new List<GUIContent>()
        {
            new GUIContent("="),
        };

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

            // Get selected variable
            Variable selectedVariable = anyVarProp.FindPropertyRelative("variable").objectReferenceValue as Variable;
            System.Type variableType = null;
            List<GUIContent> operatorsList = VariableConditionEditor.emptyList;
            if (selectedVariable != null)
            {
                variableType = selectedVariable.GetType();
                operatorsList = selectedVariable.IsComparisonSupported() ? operatorsListAll : operatorsListAssignOnly;
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
