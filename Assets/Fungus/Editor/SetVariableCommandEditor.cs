using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor (typeof(SetVariableCommand))]
public class SetVariableCommandEditor : FungusCommandEditor 
{
	public override void DrawCommandInspectorGUI()
	{
		SetVariableCommand t = target as SetVariableCommand;

		t.variableKey = EditorGUILayout.TextField(new GUIContent("Variable Key", "The name of the variable to set"), t.variableKey);
		t.booleanValue = EditorGUILayout.Toggle(new GUIContent("Boolean Value", "The boolean value to set the variable with"), t.booleanValue);
	}
}
