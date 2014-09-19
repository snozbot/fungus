using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor (typeof(Sequence))]
	public class SequenceEditor : Editor 
	{
		protected SerializedProperty sequenceNameProp;
		protected SerializedProperty descriptionProp;

		public virtual void OnEnable()
		{
			descriptionProp = serializedObject.FindProperty("description");
		}

		public virtual void DrawSequenceGUI(FungusScript fungusScript)
		{
			if (fungusScript.selectedSequence == null)
			{
				return;
			}

			serializedObject.Update();
			
			Sequence sequence = fungusScript.selectedSequence;

			EditorGUI.BeginChangeCheck();
			string sequenceName = EditorGUILayout.TextField(new GUIContent("Name", "Name of sequence object"), sequence.gameObject.name);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(sequence.gameObject, "Set Sequence Name");
				sequence.gameObject.name = sequenceName;
			}

			EditorGUILayout.PropertyField(descriptionProp);

			EditorGUILayout.Separator();

			UpdateIndentLevels(sequence);

			ReorderableListGUI.Title("Command Sequence");
			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");
			CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
			ReorderableListControl.DrawControlFromState(adaptor, null, ReorderableListFlags.HideRemoveButtons);

			if (!Application.isPlaying)
			{
				Rect copyMenuRect = GUILayoutUtility.GetLastRect();
				copyMenuRect.y += copyMenuRect.height - 17;
				copyMenuRect.width = 24;
				copyMenuRect.height = 18;
				if (GUI.Button(copyMenuRect, "", new GUIStyle("DropDown")))
				{
					ShowCopyMenu();
				}
			}

			EditorGUILayout.BeginHorizontal();

			if (fungusScript.selectedCommand != null)
			{
				CommandInfoAttribute infoAttr = CommandEditor.GetCommandInfo(fungusScript.selectedCommand.GetType());
				if (infoAttr != null)
				{
					EditorGUILayout.HelpBox(infoAttr.CommandName + ":\n" + infoAttr.HelpText, MessageType.Info, true);
				}
			}

			// Need to expand to fill space or else reorderable list width changes if no command is selected
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.EndHorizontal();

			
			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void UpdateIndentLevels(Sequence sequence)
		{
			int indentLevel = 0;
			foreach(Command command in sequence.commandList)
			{
				indentLevel += command.GetPreIndent();
				command.indentLevel = Math.Max(indentLevel, 0);
				indentLevel += command.GetPostIndent();
			}
		}

		static public void SequenceField(SerializedProperty property, GUIContent label, GUIContent nullLabel, FungusScript fungusScript)
		{
			if (fungusScript == null)
			{
				return;
			}

			Sequence sequence = property.objectReferenceValue as Sequence;
		
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
				sequence = null; // Option 'None'
			}
			else
			{
				sequence = sequences[selectedIndex - 1];
			}
			
			property.objectReferenceValue = sequence;
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

		protected void ShowCopyMenu()
		{
			bool showCut = false;
			bool showCopy = false;
			bool showDelete = false;
			bool showPaste = false;

			Sequence sequence = target as Sequence;
			foreach (Command command in sequence.commandList)
			{
				if (command.selected)
				{
					showCut = true;
					showCopy = true;
					showDelete = true;
				}
			}

			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();

			if (commandCopyBuffer.HasCommands())
			{
				showPaste = true;
			}

			GenericMenu commandMenu = new GenericMenu();
			
			commandMenu.AddItem (new GUIContent ("Select All"), false, SelectAll);
			commandMenu.AddItem (new GUIContent ("Select None"), false, SelectNone);
			commandMenu.AddSeparator("");

			if (showCut)
			{
				commandMenu.AddItem (new GUIContent ("Cut"), false, Cut);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Cut"));
			}

			if (showCopy)
			{
				commandMenu.AddItem (new GUIContent ("Copy"), false, Copy);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Copy"));
			}

			if (showPaste)
			{
				commandMenu.AddItem (new GUIContent ("Paste"), false, Paste);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Paste"));
			}

			if (showDelete)
			{
				commandMenu.AddItem (new GUIContent ("Delete"), false, Delete);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Delete"));
			}

			commandMenu.ShowAsContext();
		}

		protected virtual void SelectAll()
		{
			Sequence sequence = target as Sequence;
			foreach (Command command in sequence.commandList)
			{
				Undo.RecordObject(command, "Select All");
				command.selected = true;
			}
		}

		protected virtual void SelectNone()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();
			if (fungusScript != null)
			{
				Undo.RecordObject(fungusScript, "Select None");
				fungusScript.selectedCommand = null;
			}

			foreach (Command command in sequence.commandList)
			{
				Undo.RecordObject(command, "Select None");
				command.selected = false;
			}
		}

		protected virtual void Cut()
		{
			Copy();
			Delete();
		}

		protected virtual void Copy()
		{
			Sequence sequence = target as Sequence;

			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();

			commandCopyBuffer.Clear();

			foreach (Command command in sequence.commandList)
			{
				if (command.selected)
				{
					System.Type type = command.GetType();
					Command newCommand = Undo.AddComponent(commandCopyBuffer.gameObject, type) as Command;
					System.Reflection.FieldInfo[] fields = type.GetFields();
					foreach (System.Reflection.FieldInfo field in fields)
					{
						field.SetValue(newCommand, field.GetValue(command));
					}
					newCommand.selected = false;
				}
			}
		}

		protected virtual void Paste()
		{
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();

			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			// Find where to paste commands in sequence (either at end or after selected command)
			int pasteIndex = sequence.commandList.Count;
			if (fungusScript.selectedCommand != null)
			{
				for (int i = 0; i < sequence.commandList.Count; ++i)
				{
					Command command = sequence.commandList[i];
					
					if (fungusScript.selectedCommand == command)
					{
						pasteIndex = i + 1;
						break;
					}
				}
			}

			foreach (Command command in commandCopyBuffer.GetCommands())
			{
				System.Type type = command.GetType();
				Command newCommand = Undo.AddComponent(sequence.gameObject, type) as Command;
				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newCommand, field.GetValue(command));
				}
				newCommand.selected = false;
				
				Undo.RecordObject(sequence, "Paste");
				sequence.commandList.Insert(pasteIndex++, newCommand);
			}
		}

		protected virtual void Delete()
		{
			Sequence sequence = target as Sequence;

			for (int i = sequence.commandList.Count - 1; i >= 0; --i)
			{
				Command command = sequence.commandList[i];
				if (command != null &&
				    command.selected)
				{
					Undo.DestroyObjectImmediate(command);
					Undo.RecordObject(sequence, "Delete");
					sequence.commandList.RemoveAt(i);
				}
			}
		}
	}

}