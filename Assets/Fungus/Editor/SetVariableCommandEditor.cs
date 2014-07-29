using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		int index = -1;
		List<string> keys = new List<string>();
		for (int i = 0; i < sc.variables.Count; ++i)
		{
			Variable v = sc.variables[i];
			keys.Add(v.key);
			if (v.key == t.variableKey &&
			    index == -1)
			{
				index = i;
			}
		}

		index = EditorGUILayout.Popup("Variable", index, keys.ToArray());

		Variable variable = sc.variables[index];

		t.variableKey = variable.key;

		switch (variable.type)
		{
		case VariableType.Boolean:
			t.booleanValue = EditorGUILayout.Toggle(new GUIContent("Boolean Value", "The boolean value to set the variable with"), t.booleanValue);
			break;
		case VariableType.Integer:
			t.integerValue = EditorGUILayout.IntField(new GUIContent("Integer Value", "The integer value to set the variable with"), t.integerValue);
			break;
		case VariableType.Float:
			t.floatValue = EditorGUILayout.FloatField(new GUIContent("Float Value", "The float value to set the variable with"), t.floatValue);
			break;
		case VariableType.String:
			t.stringValue = EditorGUILayout.TextField(new GUIContent("String Value", "The string value to set the variable with"), t.stringValue);
			break;
		}

	}


}
