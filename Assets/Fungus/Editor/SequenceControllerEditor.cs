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
	
		GUIContent stepTimeLabel = new GUIContent("Step Time", "Minimum time to execute each step");
		t.stepTime = EditorGUILayout.FloatField(stepTimeLabel, t.stepTime);

		GUIContent startSequenceLabel = new GUIContent("Start Sequence", "Sequence to be executed when controller starts.");
		t.startSequence = SequenceEditor.SequenceField(startSequenceLabel, t, t.startSequence);

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

		if (GUILayout.Button("New Sequence"))
		{
			GameObject go = new GameObject("Sequence");
			go.transform.parent = t.transform;
			Sequence s = go.AddComponent<Sequence>();
			FungusEditorWindow fungusEditorWindow = EditorWindow.GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor") as FungusEditorWindow;
			s.nodeRect.x = fungusEditorWindow.scrollPos.x;
			s.nodeRect.y = fungusEditorWindow.scrollPos.y;
			Undo.RegisterCreatedObjectUndo(go, "Sequence");
			Selection.activeGameObject = go;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}