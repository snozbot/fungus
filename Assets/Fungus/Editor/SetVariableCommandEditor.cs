using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(SetVariableCommand))]
	public class SetVariableCommandEditor : FungusCommandEditor 
	{
		public override void DrawCommandInspectorGUI()
		{
			SetVariableCommand t = target as SetVariableCommand;

			SequenceController sc = t.GetParentSequenceController();
			if (sc == null)
			{
				return;
			}

			List<string> keys = new List<string>();
			keys.Add("<None>");
			int index = 0;
			for (int i = 0; i < sc.variables.Count; ++i)
			{
				Variable v = sc.variables[i];
				keys.Add(v.key);
				if (v.key == t.variableKey &&
				    index == 0)
				{
					index = i + 1;
				}
			}



			int newIndex = EditorGUILayout.Popup("Variable", index, keys.ToArray());

			if (newIndex != index)
			{
				Undo.RecordObject(t, "Select variable");
			}

			if (newIndex == 0)
			{
				t.variableKey = "";
				return;
			}

			Variable variable = sc.variables[newIndex - 1];

			if (t.variableKey != variable.key)
			{
				Undo.RecordObject(t, "Select variable");
				t.variableKey = variable.key;
			}

			bool booleanValue = t.booleanValue;
			int integerValue = t.integerValue;
			float floatValue = t.floatValue;
			string stringValue = t.stringValue;

			switch (variable.type)
			{
			case VariableType.Boolean:
				booleanValue = EditorGUILayout.Toggle(new GUIContent("Boolean Value", "The boolean value to set the variable with"), t.booleanValue);
				break;
			case VariableType.Integer:
				integerValue = EditorGUILayout.IntField(new GUIContent("Integer Value", "The integer value to set the variable with"), t.integerValue);
				break;
			case VariableType.Float:
				floatValue = EditorGUILayout.FloatField(new GUIContent("Float Value", "The float value to set the variable with"), t.floatValue);
				break;
			case VariableType.String:
				stringValue = EditorGUILayout.TextField(new GUIContent("String Value", "The string value to set the variable with"), t.stringValue);
				break;
			}

			if (booleanValue != t.booleanValue)
			{
				Undo.RecordObject(t, "Set boolean value");
				t.booleanValue = booleanValue;
			}
			else if (integerValue != t.integerValue)
			{
				Undo.RecordObject(t, "Set integer value");
				t.integerValue = integerValue;
			}
			else if (floatValue != t.floatValue)
			{
				Undo.RecordObject(t, "Set float value");
				t.floatValue = floatValue;
			}
			else if (stringValue != t.stringValue)
			{
				Undo.RecordObject(t, "Set string value");
				t.stringValue = stringValue;
			}

		}
	}

}
