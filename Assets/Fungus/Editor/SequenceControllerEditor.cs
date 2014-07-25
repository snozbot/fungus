using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

[CustomEditor (typeof(SequenceController))]
public class SequenceControllerEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		DrawDefaultInspector();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Open Fungus Editor"))
		{
			EditorWindow.GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}