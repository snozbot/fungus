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
		SerializedProperty stepTimeProp;
		SerializedProperty startSequenceProp;
		SerializedProperty startAutomaticallyProp;
		SerializedProperty colorCommandsProp;
		SerializedProperty variablesProp;

		void OnEnable()
		{
			stepTimeProp = serializedObject.FindProperty("stepTime");
			startSequenceProp = serializedObject.FindProperty("startSequence");
			startAutomaticallyProp = serializedObject.FindProperty("startAutomatically");
			colorCommandsProp = serializedObject.FindProperty("colorCommands");
			variablesProp = serializedObject.FindProperty("variables");
		}

		public void OnInspectorUpdate()
		{
			Repaint();
		}
		
		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			FungusScript t = target as FungusScript;

			if (Application.isPlaying)
			{
				if (t.executingSequence == null)
				{
					t.selectedCommand = null;
				}
				else
				{
					t.selectedCommand = t.executingSequence.activeCommand;
				}
			}

			EditorGUILayout.PropertyField(stepTimeProp, new GUIContent("Step Time", "Minimum time to execute each step"));

			SequenceEditor.SequenceField(startSequenceProp, 
			                             new GUIContent("Start Sequence", "Sequence to be executed when controller starts."), 
										 new GUIContent("<None>"),
			                             t);

			if (t.startSequence == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
			}

			EditorGUILayout.PropertyField(startAutomaticallyProp, new GUIContent("Start Automatically", "Start this Fungus Script when the scene starts."));

			EditorGUILayout.PropertyField(colorCommandsProp, new GUIContent("Color Commands", "Display commands using colors in editor window."));

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

			serializedObject.ApplyModifiedProperties();
		}

		public void DrawVariablesGUI()
		{
			FungusScript t = target as FungusScript;
			
			ReorderableListGUI.Title("Variables");

			FungusVariableListAdaptor adaptor = new FungusVariableListAdaptor(variablesProp, 0);
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