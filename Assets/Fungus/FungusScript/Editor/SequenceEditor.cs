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
		protected class AddCommandOperation
		{
			public Sequence sequence;
			public Type commandType;
			public int index;
		}

		protected SerializedProperty sequenceNameProp;

		public virtual void OnEnable()
		{
			sequenceNameProp = serializedObject.FindProperty("sequenceName");
		}

		public virtual void DrawInspectorGUI(FungusScript fungusScript)
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(sequenceNameProp);
			EditorGUILayout.Separator();

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void DrawCommandListGUI(FungusScript fungusScript)
		{
			serializedObject.Update();

			Sequence sequence = target as Sequence;
			UpdateIndentLevels(sequence);

			ReorderableListGUI.Title(sequence.sequenceName);
			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");
			CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
			adaptor.nodeRect = sequence.nodeRect;

			ReorderableListFlags flags = ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons;
			ReorderableListControl.DrawControlFromState(adaptor, null, flags);

			Rect bottomBoxRect = GUILayoutUtility.GetRect(sequence.nodeRect.width, 20);
			bottomBoxRect.y -= 7;
			bottomBoxRect.x += 5;
			bottomBoxRect.width -= 10;
			if (sequence.commandList.Count == 0)
			{
				bottomBoxRect.y -= 16;
			}

			GUI.backgroundColor = new Color32(200, 200, 200, 255);
			GUI.Box(bottomBoxRect, "");

			if (!Application.isPlaying &&
			    sequence == fungusScript.selectedSequence)
			{
				// Show add command button
				{
					Rect plusRect = bottomBoxRect;
					plusRect.x = plusRect.width - 19;
					plusRect.y += 2;
					plusRect.width = 32;
					plusRect.height = 32;

					if (GUI.Button(plusRect, FungusEditorResources.texAddButton))
					{
						ShowCommandMenu();
					}
				}

				// Show delete sequence button
				{
					Rect deleteRect = new Rect();
					deleteRect.x = sequence.nodeRect.width - 28;
					deleteRect.y = 7;
					deleteRect.width = 16;
					deleteRect.height = 16;
					
					if (GUI.Button(deleteRect, "", new GUIStyle("WinBtnCloseWin")))
					{
						FungusScriptWindow.DeleteSequence();
					}
				}
			}

			serializedObject.ApplyModifiedProperties();

			if (!Application.isPlaying)
			{
				if (Event.current.button == 1 && 
				    Event.current.type == EventType.MouseDown)
				{
					// Set a flag to show the context menu on the new display frame
					// This gives the sequence list display a chance to update
					FungusScriptWindow.showContextMenu = true;
					Event.current.Use();
				}
			}
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
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>(true);
			for (int i = 0; i < sequences.Length; ++i)
			{
				sequenceNames.Add(new GUIContent(sequences[i].sequenceName));
				
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


		void ShowCommandMenu()
		{
			Sequence sequence = target as Sequence;
			int index = sequence.commandList.Count;

			GenericMenu commandMenu = new GenericMenu();
			
			// Build menu list
			List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
			foreach(System.Type type in menuTypes)
			{
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object obj in attributes)
				{
					CommandInfoAttribute infoAttr = obj as CommandInfoAttribute;
					if (infoAttr != null)
					{
						AddCommandOperation commandOperation = new AddCommandOperation();
						
						commandOperation.sequence = sequence;
						commandOperation.commandType = type;
						commandOperation.index = index;
						
						commandMenu.AddItem (new GUIContent (infoAttr.Category + "/" + infoAttr.CommandName), 
						                     false, AddCommandCallback, commandOperation);
					}
				}
			}
			
			commandMenu.ShowAsContext();
		}
		
		void AddCommandCallback(object obj)
		{
			AddCommandOperation commandOperation = obj as AddCommandOperation;
			
			Sequence sequence = commandOperation.sequence;
			if (sequence == null)
			{
				return;
			}
			
			sequence.GetFungusScript().ClearSelectedCommands();
			
			Command newCommand = Undo.AddComponent(sequence.gameObject, commandOperation.commandType) as Command;
			sequence.GetFungusScript().selectedCommands.Add(newCommand);

			Undo.RecordObject(sequence, "Set command type");
			if (commandOperation.index < sequence.commandList.Count - 1)
			{
				sequence.commandList[commandOperation.index] = newCommand;
			}
			else
			{
				sequence.commandList.Add(newCommand);
			}
		}

	}

}