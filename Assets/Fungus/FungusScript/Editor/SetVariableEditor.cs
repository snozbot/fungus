using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(SetVariable))]
	public class SetVariableEditor : FungusCommandEditor 
	{
		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			SetVariable t = target as SetVariable;

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

			selectedIndex = EditorGUILayout.Popup(new GUIContent("Operator", "Arithmetic operator to use"), selectedIndex, operatorsList.ToArray());
			
			SetVariable.SetOperator setOperator = SetVariable.SetOperator.Assign;
			if (variable.GetType() == typeof(BooleanVariable) || 
			    variable.GetType() == typeof(StringVariable))
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
			else if (variable.GetType() == typeof(IntegerVariable) || 
			         variable.GetType() == typeof(FloatVariable))
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

			if (setOperator != t.setOperator)
			{
				Undo.RecordObject(t, "Set Operator");
				t.setOperator = setOperator;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("booleanData"));
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("integerData"));
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("floatData"));
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("stringData"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

}
