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
				if (v.key == t.variable.key &&
				    index == 0)
				{
					index = i + 1;
				}
			}

			int newIndex = EditorGUILayout.Popup("Variable", index, keys.ToArray());

			bool keyChanged = (t.variable.key != keys[newIndex]);

			if (keyChanged)
			{
				Undo.RecordObject(t, "Select variable");
				t.variable.key = keys[newIndex];
			}

			if (t.variable.key == "<None>")
			{
				return;
			}

			VariableType variableType = sc.variables[newIndex - 1].type;

			bool booleanValue = t.variable.booleanValue;
			int integerValue = t.variable.integerValue;
			float floatValue = t.variable.floatValue;
			string stringValue = t.variable.stringValue;

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

			if (booleanValue != t.variable.booleanValue)
			{
				Undo.RecordObject(t, "Set boolean value");
				t.variable.booleanValue = booleanValue;
			}
			else if (integerValue != t.variable.integerValue)
			{
				Undo.RecordObject(t, "Set integer value");
				t.variable.integerValue = integerValue;
			}
			else if (floatValue != t.variable.floatValue)
			{
				Undo.RecordObject(t, "Set float value");
				t.variable.floatValue = floatValue;
			}
			else if (stringValue != t.variable.stringValue)
			{
				Undo.RecordObject(t, "Set string value");
				t.variable.stringValue = stringValue;
			}
		}
	}

}
