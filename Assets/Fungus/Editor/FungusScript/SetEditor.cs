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
			serializedObject.Update();

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
