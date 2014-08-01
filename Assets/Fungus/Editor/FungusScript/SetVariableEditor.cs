using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Set))]
	public class SetVariableEditor : FungusCommandEditor 
	{
		public override void DrawCommandInspectorGUI()
		{
			Set t = target as Set;

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			VariableType variableType = VariableType.Boolean;
			string variableKey = SequenceEditor.VariableField(new GUIContent("Variable", "Variable to set"),
			                                                  fungusScript,
			                                                  t.variableKey,
			                                                  ref variableType);
	
			if (variableKey != t.variableKey)
			{
				Undo.RecordObject(t, "Set Variable Key");
				t.variableKey = variableKey;
			}

			if (t.variableKey == "<None>")
			{
				return;
			}

			bool booleanValue = t.booleanData.value;
			int integerValue = t.integerData.value;
			float floatValue = t.floatData.value;
			string stringValue = t.stringData.value;

			switch (variableType)
			{
			case VariableType.Boolean:
				booleanValue = EditorGUILayout.Toggle(new GUIContent("Boolean Value", "The boolean value to set the variable with"), booleanValue);
				break;
			case VariableType.Integer:
				integerValue = EditorGUILayout.IntField(new GUIContent("Integer Value", "The integer value to set the variable with"), integerValue);
				break;
			case VariableType.Float:
				floatValue = EditorGUILayout.FloatField(new GUIContent("Float Value", "The float value to set the variable with"), floatValue);
				break;
			case VariableType.String:
				stringValue = EditorGUILayout.TextField(new GUIContent("String Value", "The string value to set the variable with"), stringValue);
				break;
			}

			if (booleanValue != t.booleanData.value)
			{
				Undo.RecordObject(t, "Set boolean value");
				t.booleanData.value = booleanValue;
			}
			else if (integerValue != t.integerData.value)
			{
				Undo.RecordObject(t, "Set integer value");
				t.integerData.value = integerValue;
			}
			else if (floatValue != t.floatData.value)
			{
				Undo.RecordObject(t, "Set float value");
				t.floatData.value = floatValue;
			}
			else if (stringValue != t.stringData.value)
			{
				Undo.RecordObject(t, "Set string value");
				t.stringData.value = stringValue;
			}
		}
	}

}
