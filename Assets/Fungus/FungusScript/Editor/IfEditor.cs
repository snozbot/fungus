using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Fungus
{

	[CustomEditor (typeof(If))]
	public class IfEditor : CommandEditor 
	{
		SerializedProperty variableProp;
		SerializedProperty compareOperatorProp;
		SerializedProperty booleanValueProp;
		SerializedProperty integerValueProp;
		SerializedProperty floatValueProp;
		SerializedProperty stringValueProp;

		void OnEnable()
		{
			variableProp = serializedObject.FindProperty("variable");
			compareOperatorProp = serializedObject.FindProperty("compareOperator");
			booleanValueProp = serializedObject.FindProperty("booleanValue");
			integerValueProp = serializedObject.FindProperty("integerValue");
			floatValueProp = serializedObject.FindProperty("floatValue");
			stringValueProp = serializedObject.FindProperty("stringValue");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			If t = target as If;

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			VariableEditor.VariableField(variableProp, 
			                             new GUIContent("Variable", "Variable to use in operation"),
										 t.GetFungusScript(),
										 null);

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
				EditorGUILayout.PropertyField(booleanValueProp);
			}
			else if (variableType == typeof(IntegerVariable))
			{
				EditorGUILayout.PropertyField(integerValueProp);
			}
			else if (variableType == typeof(FloatVariable))
			{
				EditorGUILayout.PropertyField(floatValueProp);
			}
			else if (variableType == typeof(StringVariable))
			{
				EditorGUILayout.PropertyField(stringValueProp);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

}
