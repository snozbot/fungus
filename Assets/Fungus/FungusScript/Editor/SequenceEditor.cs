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
		protected class SetEventHandlerOperation
		{
			public Sequence sequence;
			public Type eventHandlerType;
		}

		protected class AddCommandOperation
		{
			public Sequence sequence;
			public Type commandType;
			public int index;
		}

		public virtual void DrawSequenceGUI(FungusScript fungusScript)
		{
			serializedObject.Update();

			Sequence sequence = target as Sequence;

			SerializedProperty descriptionProp = serializedObject.FindProperty("description");
			EditorGUILayout.PropertyField(descriptionProp);

			SerializedProperty runSlowInEditorProp = serializedObject.FindProperty("runSlowInEditor");
			EditorGUILayout.PropertyField(runSlowInEditorProp);

			DrawEventHandlerGUI(fungusScript);

			UpdateIndentLevels(sequence);

			SerializedProperty sequenceNameProperty = serializedObject.FindProperty("sequenceName");
			Rect sequenceLabelRect = new Rect(45, 5, 120, 16);
			EditorGUI.LabelField(sequenceLabelRect, new GUIContent("Sequence Name"));
			Rect sequenceNameRect = new Rect(45, 21, 180, 16);
			EditorGUI.PropertyField(sequenceNameRect, sequenceNameProperty, new GUIContent(""));

			// Ensure sequence name is unique for this Fungus Script
			string uniqueName = fungusScript.GetUniqueSequenceKey(sequenceNameProperty.stringValue, sequence);
			if (uniqueName != sequence.sequenceName)
			{
				sequenceNameProperty.stringValue = uniqueName;
			}

			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");

			ReorderableListGUI.Title("Commands");
			CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
			adaptor.nodeRect = sequence.nodeRect;

			ReorderableListFlags flags = ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.DisableContextMenu;

			ReorderableListControl.DrawControlFromState(adaptor, null, flags);

			if (Event.current.type == EventType.ContextClick)
			{
				ShowContextMenu();
			}

			if (sequence == fungusScript.selectedSequence)
			{
				// Show add command button
				{
					GUILayout.BeginHorizontal();

					GUILayout.FlexibleSpace();

					if (GUILayout.Button(FungusEditorResources.texAddButton))
					{
						ShowCommandMenu();
					}

					GUILayout.EndHorizontal();
				}
			}

			// Remove any null entries in the command list.
			// This can happen when a command class is deleted or renamed.
			for (int i = commandListProperty.arraySize - 1; i >= 0; --i)
			{
				SerializedProperty commandProperty = commandListProperty.GetArrayElementAtIndex(i);
				if (commandProperty.objectReferenceValue == null)
				{
					commandListProperty.DeleteArrayElementAtIndex(i);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawEventHandlerGUI(FungusScript fungusScript)
		{
			// Show available Event Handlers in a drop down list with type of current
			// event handler selected.
			List<System.Type> eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).ToList();

			Sequence sequence = target as Sequence;
			System.Type currentType = null;
			if (sequence.eventHandler != null)
			{
				currentType = sequence.eventHandler.GetType();
			}

			string currentHandlerName = "<None>";
			if (currentType != null)
			{
				EventHandlerInfoAttribute info = EventHandler.GetEventHandlerInfo(currentType);
				currentHandlerName = info.EventHandlerName;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent("Execute On Event"));
			if (GUILayout.Button(new GUIContent(currentHandlerName), EditorStyles.popup))
			{
				SetEventHandlerOperation noneOperation = new SetEventHandlerOperation();
				noneOperation.sequence = sequence;
				noneOperation.eventHandlerType = null;
				
				GenericMenu eventHandlerMenu = new GenericMenu();
				eventHandlerMenu.AddItem(new GUIContent("None"), false, OnSelectEventHandler, noneOperation);

				// Add event handlers with no category first
				foreach (System.Type type in eventHandlerTypes)
				{
					EventHandlerInfoAttribute info = EventHandler.GetEventHandlerInfo(type);					
					if (info.Category.Length == 0)
					{
						SetEventHandlerOperation operation = new SetEventHandlerOperation();
						operation.sequence = sequence;
						operation.eventHandlerType = type;
						
						eventHandlerMenu.AddItem(new GUIContent(info.EventHandlerName), false, OnSelectEventHandler, operation);
					}
				}

				// Add event handlers with a category afterwards
				foreach (System.Type type in eventHandlerTypes)
				{
					EventHandlerInfoAttribute info = EventHandler.GetEventHandlerInfo(type);					
					if (info.Category.Length > 0)
					{			
						SetEventHandlerOperation operation = new SetEventHandlerOperation();
						operation.sequence = sequence;
						operation.eventHandlerType = type;
						string typeName = info.Category + "/" + info.EventHandlerName;
						eventHandlerMenu.AddItem(new GUIContent(typeName), false, OnSelectEventHandler, operation);
					}
				}


				eventHandlerMenu.ShowAsContext();
			}
			EditorGUILayout.EndHorizontal();

			if (sequence.eventHandler != null)
			{
				EventHandlerEditor eventHandlerEditor = Editor.CreateEditor(sequence.eventHandler) as EventHandlerEditor;
				eventHandlerEditor.DrawInspectorGUI();
				DestroyImmediate(eventHandlerEditor);
			}
		}

		protected void OnSelectEventHandler(object obj)
		{
			SetEventHandlerOperation operation = obj as SetEventHandlerOperation;
			Sequence sequence = operation.sequence;
			System.Type selectedType = operation.eventHandlerType;
			if (sequence == null)
			{
				return;
			}

			Undo.RecordObject(sequence, "Set Start Event");

			if (sequence.eventHandler != null)
			{
				Undo.DestroyObjectImmediate(sequence.eventHandler);
			}

			if (selectedType != null)
			{
				EventHandler newHandler = Undo.AddComponent(sequence.gameObject, selectedType) as EventHandler;
				newHandler.parentSequence = sequence;
				sequence.eventHandler = newHandler;
			}
		}

		protected virtual void UpdateIndentLevels(Sequence sequence)
		{
			int indentLevel = 0;
			foreach(Command command in sequence.commandList)
			{
				if (command == null)
				{
					continue;
				}

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

		// Compare delegate for sorting the list of command attributes
		static int CompareCommandAttributes(KeyValuePair<System.Type, CommandInfoAttribute> x, KeyValuePair<System.Type, CommandInfoAttribute> y)
		{
			int compare = (x.Value.Category.CompareTo(y.Value.Category));
			if (compare == 0)
			{
				compare = (x.Value.CommandName.CompareTo(y.Value.CommandName));
			}
			return compare;
		}

		void ShowCommandMenu()
		{
			Sequence sequence = target as Sequence;
			int index = sequence.commandList.Count;

			GenericMenu commandMenu = new GenericMenu();
			
			// Build menu list
			List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
			List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);

			filteredAttributes.Sort( CompareCommandAttributes );

			foreach(var keyPair in filteredAttributes)
			{
				AddCommandOperation commandOperation = new AddCommandOperation();
				
				commandOperation.sequence = sequence;
				commandOperation.commandType = keyPair.Key;
				commandOperation.index = index;

				GUIContent menuItem;
				if (keyPair.Value.Category == "")
				{
					menuItem = new GUIContent(keyPair.Value.CommandName);
				}
				else
				{
					menuItem = new GUIContent (keyPair.Value.Category + "/" + keyPair.Value.CommandName);
				}

				commandMenu.AddItem(menuItem, false, AddCommandCallback, commandOperation);
			}

			commandMenu.ShowAsContext();
		}

		List<KeyValuePair<System.Type,CommandInfoAttribute>> GetFilteredCommandInfoAttribute(List<System.Type> menuTypes)
		{
			Dictionary<string, KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = new Dictionary<string, KeyValuePair<System.Type, CommandInfoAttribute>>();
			
			foreach (System.Type type in menuTypes)
			{
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object obj in attributes)
				{
					CommandInfoAttribute infoAttr = obj as CommandInfoAttribute;
					if (infoAttr != null)
					{
						string dictionaryName = string.Format("{0}/{1}", infoAttr.Category, infoAttr.CommandName);
						
						int existingItemPriority = -1;
						if (filteredAttributes.ContainsKey(dictionaryName))
						{
							existingItemPriority = filteredAttributes[dictionaryName].Value.Priority;
						}
						
						if (infoAttr.Priority > existingItemPriority)
						{
							KeyValuePair<System.Type, CommandInfoAttribute> keyValuePair = new KeyValuePair<System.Type, CommandInfoAttribute>(type, infoAttr);
							filteredAttributes[dictionaryName] = keyValuePair;
						}
					}
				}
			}
			return filteredAttributes.Values.ToList<KeyValuePair<System.Type,CommandInfoAttribute>>();
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
			sequence.GetFungusScript().AddSelectedCommand(newCommand);
			newCommand.parentSequence = sequence;

			// Let command know it has just been added to the sequence
			newCommand.OnCommandAdded(sequence);

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

		public void ShowContextMenu()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			if (fungusScript == null)
			{
				return;
			}
			
			bool showCut = false;
			bool showCopy = false;
			bool showDelete = false;
			bool showPaste = false;
			
			if (fungusScript.selectedCommands.Count > 0)
			{
				showCut = true;
				showCopy = true;
				showDelete = true;
			}
			
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			
			if (commandCopyBuffer.HasCommands())
			{
				showPaste = true;
			}
			
			GenericMenu commandMenu = new GenericMenu();
			
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
			
			commandMenu.AddSeparator("");
			
			commandMenu.AddItem (new GUIContent ("Select All"), false, SelectAll);
			commandMenu.AddItem (new GUIContent ("Select None"), false, SelectNone);
			
			commandMenu.ShowAsContext();
		}
		
		protected void SelectAll()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}
			
			fungusScript.ClearSelectedCommands();
			Undo.RecordObject(fungusScript, "Select All");
			foreach (Command command in fungusScript.selectedSequence.commandList)
			{
				fungusScript.AddSelectedCommand(command);
			}

			Repaint();
		}
		
		protected void SelectNone()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}
			
			Undo.RecordObject(fungusScript, "Select None");
			fungusScript.ClearSelectedCommands();

			Repaint();
		}
		
		protected void Cut()
		{
			Copy();
			Delete();
		}
		
		protected void Copy()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}
			
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			commandCopyBuffer.Clear();

			// Scan through all commands in execution order to see if each needs to be copied
			foreach (Command command in fungusScript.selectedSequence.commandList)
			{
				if (fungusScript.selectedCommands.Contains(command))
				{
					System.Type type = command.GetType();
					Command newCommand = Undo.AddComponent(commandCopyBuffer.gameObject, type) as Command;
					System.Reflection.FieldInfo[] fields = type.GetFields();
					foreach (System.Reflection.FieldInfo field in fields)
					{
						field.SetValue(newCommand, field.GetValue(command));
					}
				}
			}
		}
		
		protected void Paste()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}
			
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			
			// Find where to paste commands in sequence (either at end or after last selected command)
			int pasteIndex = fungusScript.selectedSequence.commandList.Count;
			if (fungusScript.selectedCommands.Count > 0)
			{
				for (int i = 0; i < fungusScript.selectedSequence.commandList.Count; ++i)
				{
					Command command = fungusScript.selectedSequence.commandList[i];
					
					foreach (Command selectedCommand in fungusScript.selectedCommands)
					{
						if (command == selectedCommand)
						{
							pasteIndex = i + 1;
						}
					}
				}
			}
			
			foreach (Command command in commandCopyBuffer.GetCommands())
			{
				System.Type type = command.GetType();
				Command newCommand = Undo.AddComponent(fungusScript.selectedSequence.gameObject, type) as Command;
				newCommand.parentSequence = fungusScript.selectedSequence;

				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newCommand, field.GetValue(command));
				}
				
				Undo.RecordObject(fungusScript.selectedSequence, "Paste");
				fungusScript.selectedSequence.commandList.Insert(pasteIndex++, newCommand);
			}

			Repaint();
		}
		
		protected void Delete()
		{
			Sequence sequence = target as Sequence;
			FungusScript fungusScript = sequence.GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}
			
			for (int i = fungusScript.selectedSequence.commandList.Count - 1; i >= 0; --i)
			{
				Command command = fungusScript.selectedSequence.commandList[i];
				foreach (Command selectedCommand in fungusScript.selectedCommands)
				{
					if (command == selectedCommand)
					{
						Undo.RecordObject(fungusScript.selectedSequence, "Delete");
						fungusScript.selectedSequence.commandList.RemoveAt(i);
						Undo.DestroyObjectImmediate(command);
						
						break;
					}
				}
			}
			
			Undo.RecordObject(fungusScript, "Delete");
			fungusScript.ClearSelectedCommands();

			Repaint();
		}
	}

}