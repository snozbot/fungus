using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Set))]
	public class SetEditor : FungusCommandEditor 
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
			
			List<GUIContent> operatorsList = new List<GUIContent>();
			operatorsList.Add(new GUIContent("="));
			if (variableType == VariableType.Boolean)
			{
				operatorsList.Add(new GUIContent("!"));
			}
			else if (variableType == VariableType.Integer ||
			         variableType == VariableType.Float)
			{
					operatorsList.Add(new GUIContent("+"));
					operatorsList.Add(new GUIContent("-"));
					operatorsList.Add(new GUIContent("*"));
					operatorsList.Add(new GUIContent("/"));
			}
			
			int selectedIndex = 0;
			switch (t.setOperator)
			{
				default:
				case Set.SetOperator.Assign:
					selectedIndex = 0;
					break;
				case Set.SetOperator.Negate:
					selectedIndex = 1;
					break;
				case Set.SetOperator.Add:
					selectedIndex = 1;
					break;
				case Set.SetOperator.Subtract:
					selectedIndex = 2;
					break;
				case Set.SetOperator.Multiply:
					selectedIndex = 3;
					break;
				case Set.SetOperator.Divide:
					selectedIndex = 4;
					break;
			}

			selectedIndex = EditorGUILayout.Popup(new GUIContent("Operator", "Arithmetic operator to use"), selectedIndex, operatorsList.ToArray());
			
			Set.SetOperator setOperator = Set.SetOperator.Assign;
			switch (variableType)
			{
			default:
			case VariableType.Boolean:
			case VariableType.String:
				switch (selectedIndex)
				{
				default:
				case 0:
					setOperator = Set.SetOperator.Assign;
					break;
				case 1:
					setOperator = Set.SetOperator.Negate;
					break;
				}
				break;
			case VariableType.Integer:
			case VariableType.Float:
				switch (selectedIndex)
				{
				default:
				case 0:
					setOperator = Set.SetOperator.Assign;
					break;
				case 1:
					setOperator = Set.SetOperator.Add;
					break;
				case 2:
					setOperator = Set.SetOperator.Subtract;
					break;
				case 3:
					setOperator = Set.SetOperator.Multiply;
					break;
				case 4:
					setOperator = Set.SetOperator.Divide;
					break;
				}
				break;
			}

			if (setOperator != t.setOperator)
			{
				Undo.RecordObject(t, "Set Operator");
				t.setOperator = setOperator;
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
