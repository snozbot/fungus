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
        protected struct VariablePropertyInfo
        {
            public string name;
            public SerializedProperty dataProp;

            public VariablePropertyInfo(string name, SerializedProperty dataProp) {
                this.name = name;
                this.dataProp = dataProp;
            }
        }

        protected SerializedProperty variableProp;
        protected SerializedProperty setOperatorProp;

        // Variable data props
        protected Dictionary<System.Type, VariablePropertyInfo> propertyInfoByVariableType;

        protected List<SerializedProperty> variableDataProps;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            variableProp = serializedObject.FindProperty("variable");
            setOperatorProp = serializedObject.FindProperty("setOperator");

            // Get variable data props by name
            propertyInfoByVariableType = new Dictionary<System.Type, VariablePropertyInfo>() {
                { typeof(BooleanVariable), new VariablePropertyInfo("Boolean", serializedObject.FindProperty("booleanData")) },
                { typeof(IntegerVariable), new VariablePropertyInfo("Integer", serializedObject.FindProperty("integerData")) },
                { typeof(FloatVariable), new VariablePropertyInfo("Float", serializedObject.FindProperty("floatData")) },
                { typeof(StringVariable), new VariablePropertyInfo("String", serializedObject.FindProperty("stringData")) },
                { typeof(GameObjectVariable), new VariablePropertyInfo("GameObject", serializedObject.FindProperty("gameObjectData")) }
            };
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
            SetOperator[] setOperators = SetVariable.operatorsByVariableType[variableType];

            // Create operator list
            List<GUIContent> operatorsList = new List<GUIContent>();
            foreach (var setOperator in setOperators)
            {
                switch (setOperator)
                {
                    case SetOperator.Assign:
                        operatorsList.Add(new GUIContent("="));
                        break;
                    case SetOperator.Negate:
                        operatorsList.Add(new GUIContent("=!"));
                        break;
                    case SetOperator.Add:
                        operatorsList.Add(new GUIContent("+="));
                        break;
                    case SetOperator.Subtract:
                        operatorsList.Add(new GUIContent("-="));
                        break;
                    case SetOperator.Multiply:
                        operatorsList.Add(new GUIContent("*="));
                        break;
                    case SetOperator.Divide:
                        operatorsList.Add(new GUIContent("\\="));
                        break;
                    default:
                        Debug.LogError("The " + setOperator.ToString() + " operator has no matching GUIContent.");
                        break;
                }
            }
            
            // Get previously selected operator
            int selectedIndex = System.Array.IndexOf(setOperators, t._SetOperator);
            if (selectedIndex < 0)
            {
                // Default to first index if the operator is not found in the available operators list
                // This can occur when changing between variable types
                selectedIndex = 0;
            }

            // Get next selected operator
            selectedIndex = EditorGUILayout.Popup(new GUIContent("Operation", "Arithmetic operator to use"), selectedIndex, operatorsList.ToArray());
            

            setOperatorProp.enumValueIndex = (int)setOperators[selectedIndex];


            VariablePropertyInfo propertyInfo = propertyInfoByVariableType[variableType];
            EditorGUILayout.PropertyField(propertyInfo.dataProp, new GUIContent(propertyInfo.name));


            serializedObject.ApplyModifiedProperties();
        }
    }
}
