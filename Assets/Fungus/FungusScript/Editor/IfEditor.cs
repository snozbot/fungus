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
		protected SerializedProperty variableProp;
		protected SerializedProperty compareOperatorProp;
		protected SerializedProperty booleanDataProp;
		protected SerializedProperty integerDataProp;
		protected SerializedProperty floatDataProp;
		protected SerializedProperty stringDataProp;

		protected virtual void OnEnable()
		{
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
