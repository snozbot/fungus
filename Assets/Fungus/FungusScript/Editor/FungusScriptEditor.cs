using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;

namespace Fungus
{
	[CustomEditor (typeof(FungusScript))]
	public class FungusScriptEditor : Editor 
	{
		SerializedProperty stepTimeProp;
		SerializedProperty startSequenceProp;
		SerializedProperty startAutomaticallyProp;
		SerializedProperty colorCommandsProp;
		SerializedProperty showSequenceObjectsProp;
		SerializedProperty variablesProp;

		void OnEnable()
		{
			stepTimeProp = serializedObject.FindProperty("stepTime");
			startSequenceProp = serializedObject.FindProperty("startSequence");
			startAutomaticallyProp = serializedObject.FindProperty("startAutomatically");
			colorCommandsProp = serializedObject.FindProperty("colorCommands");
			showSequenceObjectsProp = serializedObject.FindProperty("showSequenceObjects");
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

			t.UpdateHideFlags();

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

			EditorGUILayout.PropertyField(showSequenceObjectsProp, new GUIContent("Show Sequence Objects", "Display the child Sequence game objects in the hierarchy view."));

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
				CommandEditor commandEditor = Editor.CreateEditor(t.selectedCommand) as CommandEditor;
				commandEditor.DrawCommandInspectorGUI();
				DestroyImmediate(commandEditor);
			}

			EditorGUILayout.Separator();

			DrawVariablesGUI();

			serializedObject.ApplyModifiedProperties();
		}

		public void DrawVariablesGUI()
		{
			FungusScript t = target as FungusScript;
			
			ReorderableListGUI.Title("Variables");

			VariableListAdaptor adaptor = new VariableListAdaptor(variablesProp, 0);
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
		
		void AddVariable<T>(object obj) where T : Variable
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			Undo.RecordObject(fungusScript, "Add Variable");
			T newVariable = fungusScript.gameObject.AddComponent<T>();
			newVariable.key = fungusScript.GetUniqueVariableKey("");
			fungusScript.variables.Add(newVariable);
		}
	}
	
}