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

			FungusVariable variable = FungusVariableEditor.VariableField(new GUIContent("Variable", "Variable to set"),
			                                                             fungusScript,
			                                                             t.variable);

			if (variable != t.variable)
			{
				Undo.RecordObject(t, "Set Variable Key");
				t.variable = variable;
			}

			if (t.variable == null)
			{
				return;
			}
			
			List<GUIContent> operatorsList = new List<GUIContent>();
			operatorsList.Add(new GUIContent("="));
			if (variable.GetType() == typeof(BooleanVariable))
			{
				operatorsList.Add(new GUIContent("!"));
			}
			else if (variable.GetType() == typeof(IntegerVariable) ||
			         variable.GetType() == typeof(FloatVariable))
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
			if (variable.GetType() == typeof(BooleanVariable) || 
			    variable.GetType() == typeof(StringVariable))
			{
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
			} 
			else if (variable.GetType() == typeof(IntegerVariable) || 
			         variable.GetType() == typeof(FloatVariable))
			{
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
			}

			if (setOperator != t.setOperator)
			{
				Undo.RecordObject(t, "Set Operator");
				t.setOperator = setOperator;
			}

			FungusVariable setVariable = FungusVariableEditor.VariableField(new GUIContent("Other Variable", "The variable to read the assigned value from."),
			                                                                t.GetFungusScript(),
			                                                                t.setVariable,
			                                                                v => v.GetType() == variable.GetType ());

			if (setVariable != t.setVariable)
			{
				Undo.RecordObject(t, "Select Set Variable");
				t.setVariable = setVariable;
			}

			if (setVariable == null)
			{
				EditorGUI.BeginChangeCheck();

				bool booleanValue = t.booleanValue;
				int integerValue = t.integerValue;
				float floatValue = t.floatValue;
				string stringValue = t.stringValue;

				if (variable.GetType() == typeof(BooleanVariable))
				{
					booleanValue = EditorGUILayout.Toggle (new GUIContent("Boolean Value", "A constant boolean value to set the variable with"), booleanValue);
				}
				else if (variable.GetType() == typeof(IntegerVariable))
				{
					integerValue = EditorGUILayout.IntField(new GUIContent("Integer Value", "A constant integer value to set the variable with"), integerValue);
				}
				else if (variable.GetType() == typeof(FloatVariable))
				{
					floatValue = EditorGUILayout.FloatField(new GUIContent("Float Value", "A constant float value to set the variable with"), floatValue);
				}
				else if (variable.GetType() == typeof(StringVariable))
				{
					stringValue = EditorGUILayout.TextField(new GUIContent("String Value", "A constant string value to set the variable with"), stringValue);
				}

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(t, "Set Variable Data");

					t.booleanValue = booleanValue;
					t.integerValue = integerValue;
					t.floatValue = floatValue;
					t.stringValue = stringValue;
				}
			}
		}
	}

}
