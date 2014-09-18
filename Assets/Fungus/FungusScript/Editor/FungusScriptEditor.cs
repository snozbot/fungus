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
		protected SerializedProperty startSequenceProp;
		protected SerializedProperty executeOnStartProp;
		protected SerializedProperty settingsProp;
		protected SerializedProperty variablesProp;

		protected virtual void OnEnable()
		{
			startSequenceProp = serializedObject.FindProperty("startSequence");
			executeOnStartProp = serializedObject.FindProperty("executeOnStart");
			settingsProp = serializedObject.FindProperty("settings");
			variablesProp = serializedObject.FindProperty("variables");
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

			SequenceEditor.SequenceField(startSequenceProp, 
			                             new GUIContent("Start Sequence", "First sequence to execute when the Fungus Script executes"), 
										 new GUIContent("<None>"),
			                             t);

			if (t.startSequence == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
			}

			EditorGUILayout.PropertyField(executeOnStartProp, new GUIContent("Execute On Start", "Execute this Fungus Script when the scene starts playing"));

			EditorGUILayout.PropertyField(settingsProp, new GUIContent("Settings", "Configution options for the Fungus Script"), true);

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

		public virtual void DrawVariablesGUI()
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
		
		protected virtual void AddVariable<T>(object obj) where T : Variable
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