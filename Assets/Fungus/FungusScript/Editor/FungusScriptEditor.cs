using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;
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

		public void DrawSequenceGUI(FungusScript fungusScript)
		{
			if (fungusScript.selectedSequence == null)
			{
				return;
			}
				
			Sequence sequence = fungusScript.selectedSequence;

			EditorGUI.BeginChangeCheck();

			string name = EditorGUILayout.TextField(new GUIContent("Name", "Name of sequence displayed in editor window"), sequence.name);
			string desc = EditorGUILayout.TextField(new GUIContent("Description", "Sequence description displayed in editor window"), sequence.description);
	
			EditorGUILayout.Separator();

			if (name != sequence.name)
			{
				// The name is the gameobject name, so have to undo seperately
				Undo.RecordObject(sequence.gameObject, "Set Sequence Name");
				sequence.name = name;
			}

			if (desc != sequence.description)
			{
				Undo.RecordObject(sequence, "Set Sequence Description");
				sequence.description = desc;
			}

			GUILayout.Box("Commands", GUILayout.ExpandWidth(true));

			FungusCommand[] commands = sequence.GetComponents<FungusCommand>();
			foreach (FungusCommand command in commands)
			{
				FungusCommandEditor commandEditor = Editor.CreateEditor(command) as FungusCommandEditor;
				commandEditor.DrawCommandRowGUI();
			}

			if (Application.isPlaying)
			{
				return;
			}

			EditorGUILayout.Separator();
			
			GUILayout.Box("New Command", GUILayout.ExpandWidth(true));

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();

			List<string> commandNames = new List<string>();
			List<System.Type> commandTypes = EditorExtensions.FindDerivedTypes(typeof(FungusCommand)).ToList();

			foreach (System.Type type in commandTypes)
			{
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object obj in attributes)
				{
					CommandCategoryAttribute categoryAttr = obj as CommandCategoryAttribute;
					if (categoryAttr != null)
					{
						string commandItem = categoryAttr.Category + " / " + GetCommandName(type);
						commandNames.Add(commandItem);
						break;
					}
				}
			}
			
			int selectedCommandIndex = EditorGUILayout.Popup(fungusScript.selectedAddCommandIndex, commandNames.ToArray());

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(fungusScript, "Select Command");
				fungusScript.selectedAddCommandIndex = selectedCommandIndex;
			}

			if (selectedCommandIndex >= commandTypes.Count)
			{
				EditorGUILayout.EndHorizontal();
				return;
			}

			System.Type selectedType = commandTypes[selectedCommandIndex];
			if (fungusScript.selectedSequence == null ||
				selectedType == null)
			{
				EditorGUILayout.EndHorizontal();
				return;
			}

			if (GUILayout.Button(new GUIContent("Add", "Add the selected command to the sequence"), EditorStyles.miniButton))
			{
				Undo.AddComponent(fungusScript.selectedSequence.gameObject, selectedType);
			}
			
			EditorGUILayout.EndHorizontal();

			object[] helpAttributes = selectedType.GetCustomAttributes(typeof(HelpTextAttribute), false);
			foreach (object obj in helpAttributes)
			{
				HelpTextAttribute helpTextAttr = obj as HelpTextAttribute;
				if (helpTextAttr != null)
				{
					GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
					labelStyle.wordWrap = true;
					EditorGUILayout.HelpBox(helpTextAttr.HelpText, MessageType.Info);
					break;
				}
			}

			EditorGUILayout.Separator();
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

		public static string GetCommandName(System.Type commandType)
		{
			object[] attributes = commandType.GetCustomAttributes(typeof(CommandNameAttribute), false);
			foreach (object obj in attributes)
			{
				CommandNameAttribute commandNameAttr = obj as CommandNameAttribute;
				if (commandNameAttr != null)
				{
					return commandNameAttr.CommandName;
				}
			}
			
			return commandType.Name;
		}
	}
	
}