using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rotorz.ReorderableList;

namespace Fungus.Script
{

	[CustomEditor (typeof(Sequence))]
	public class SequenceEditor : Editor 
	{
		public void DrawSequenceGUI(FungusScript fungusScript)
		{
			if (fungusScript.selectedSequence == null)
			{
				return;
			}

			serializedObject.Update();
			
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
			

			ReorderableListGUI.Title("Command Sequence");

			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");
			FungusCommandListAdaptor adaptor = new FungusCommandListAdaptor(commandListProperty, 0);
			ReorderableListControl.DrawControlFromState(adaptor, null, ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton);

			if (Application.isPlaying)
			{
				return;
			}
			
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Separator();

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
						string commandItem = categoryAttr.Category + " - " + FungusScriptEditor.GetCommandName(type);
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
			
			if (GUILayout.Button(new GUIContent("Add Command", "Add the selected command to the sequence"), EditorStyles.miniButton))
			{
				FungusCommand newCommand = Undo.AddComponent(fungusScript.selectedSequence.gameObject, selectedType) as FungusCommand;
				Undo.RecordObject(fungusScript, "Add Command");
				fungusScript.selectedSequence.commandList.Add(newCommand);
			}

			if (fungusScript.copyCommand != null)
			{
				if (GUILayout.Button("Paste", EditorStyles.miniButton))
				{
					FungusCommandEditor.PasteCommand(fungusScript.copyCommand, fungusScript.selectedSequence);
				}
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

			serializedObject.ApplyModifiedProperties();
		}

		static public Sequence SequenceField(GUIContent label, GUIContent nullLabel, FungusScript fungusScript, Sequence sequence)
		{
			if (fungusScript == null)
			{
				return null;
			}
			
			Sequence result = sequence;
			
			// Build dictionary of child sequences
			List<GUIContent> sequenceNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			sequenceNames.Add(nullLabel);
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();
			for (int i = 0; i < sequences.Length; ++i)
			{
				sequenceNames.Add(new GUIContent(sequences[i].name));
				
				if (sequence == sequences[i])
				{
					selectedIndex = i + 1;
				}
			}
			
			selectedIndex = EditorGUILayout.Popup(label, selectedIndex, sequenceNames.ToArray());
			if (selectedIndex == 0)
			{
				result = null; // Option 'None'
			}
			else
			{
				result = sequences[selectedIndex - 1];
			}
			
			return result;
		}

		static public Sequence SequenceField(Rect position, GUIContent nullLabel, FungusScript fungusScript, Sequence sequence)
		{
			if (fungusScript == null)
			{
				return null;
			}
			
			Sequence result = sequence;
			
			// Build dictionary of child sequences
			List<GUIContent> sequenceNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			sequenceNames.Add(nullLabel);
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();
			for (int i = 0; i < sequences.Length; ++i)
			{
				sequenceNames.Add(new GUIContent(sequences[i].name));
				
				if (sequence == sequences[i])
				{
					selectedIndex = i + 1;
				}
			}
			
			selectedIndex = EditorGUI.Popup(position, selectedIndex, sequenceNames.ToArray());
			if (selectedIndex == 0)
			{
				result = null; // Option 'None'
			}
			else
			{
				result = sequences[selectedIndex - 1];
			}
			
			return result;
		}
	}

}