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
		SequenceController t = target as SequenceController;
	
		t.stepTime = EditorGUILayout.FloatField("Step Time", t.stepTime);

		t.startSequence = SequenceEditor.SequenceField("Start Sequence", t, t.startSequence);

		if (t.startSequence == null)
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = new Color(1,0,0);
			EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
		}

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