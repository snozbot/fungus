using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;

namespace Fungus.Script
{
	[CustomEditor (typeof(FungusScript))]
	public class FungusScriptEditor : Editor 
	{
		public void OnInspectorUpdate()
		{
			Repaint();
		}
		
		public override void OnInspectorGUI() 
		{
			FungusScript t = target as FungusScript;

			if (t != null)
			{
				t.transform.hideFlags = HideFlags.None;
			}

			if (Application.isPlaying)
			{
				t.selectedCommand = t.executingSequence.activeCommand;
			}

			EditorGUI.BeginChangeCheck();

			float stepTime = EditorGUILayout.FloatField(new GUIContent("Step Time", "Minimum time to execute each step"), t.stepTime);

			Sequence startSequence = SequenceEditor.SequenceField(new GUIContent("Start Sequence", "Sequence to be executed when controller starts."), 
			                                                      new GUIContent("<None>"),
			                                                      t, 
			                                                      t.startSequence);
			if (t.startSequence == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
			}

			bool startAutomatically = EditorGUILayout.Toggle(new GUIContent("Start Automatically", "Start this Fungus Script when the scene starts."), t.startAutomatically);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Fungus Script");
				t.stepTime = stepTime;
				t.startSequence = startSequence;
				t.startAutomatically = startAutomatically;
			}

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open Editor"))
			{
				EditorWindow.GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			if (t.selectedCommand != null)
			{
				FungusCommandEditor commandEditor = Editor.CreateEditor(t.selectedCommand) as FungusCommandEditor;
				commandEditor.DrawCommandInspectorGUI();
			}

			EditorGUILayout.Separator();

			DrawVariablesGUI();
		}

		public void DrawVariablesGUI()
		{
			serializedObject.Update();
			
			FungusScript t = target as FungusScript;
			
			ReorderableListGUI.Title("Variables");

			SerializedProperty variablesProperty = serializedObject.FindProperty("variables");
			FungusVariableListAdaptor adaptor = new FungusVariableListAdaptor(variablesProperty, 0);
			ReorderableListControl.DrawControlFromState(adaptor, null, ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton);
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			if (!Application.isPlaying && GUILayout.Button("Add Variable"))
			{
				GenericMenu menu = new GenericMenu ();
				
				menu.AddItem(new GUIContent ("Boolean"), false, AddVariable<BooleanVariable>, t);
				menu.AddItem (new GUIContent ("Integer"), false, AddVariable<IntegerVariable>, t);
				menu.AddItem (new GUIContent ("Float"), false, AddVariable<FloatVariable>, t);
				menu.AddItem (new GUIContent ("String"), false, AddVariable<StringVariable>, t);
				
				menu.ShowAsContext ();
			}
			GUILayout.EndHorizontal();
			
			serializedObject.ApplyModifiedProperties();
		}
		
		void AddVariable<T>(object obj) where T : FungusVariable
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			Undo.RecordObject(fungusScript, "Add Variable");
			T variable = fungusScript.gameObject.AddComponent<T>();
			variable.key = fungusScript.GetUniqueVariableKey("");
			fungusScript.variables.Add(variable);
		}
	}
	
}