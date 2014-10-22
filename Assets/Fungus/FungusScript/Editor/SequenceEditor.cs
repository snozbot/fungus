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

		public virtual void DrawInspectorGUI(FungusScript fungusScript)
		{
			serializedObject.Update();

			// We acquire the serialized properties in the draw methods instead of in OnEnable as otherwise
			// deleting or renaming a command class would generate a bunch of null reference exceptions.
			SerializedProperty sequenceNameProp = serializedObject.FindProperty("sequenceName");

			EditorGUILayout.PropertyField(sequenceNameProp);
			EditorGUILayout.Separator();

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void DrawCommandListGUI(FungusScript fungusScript)
		{
			serializedObject.Update();

			Sequence sequence = target as Sequence;
			UpdateIndentLevels(sequence);

			sequence.nodeRect.width = 240;

			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");

			ReorderableListGUI.Title(sequence.sequenceName);
			CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
			adaptor.nodeRect = sequence.nodeRect;

			ReorderableListFlags flags = ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.DisableContextMenu;
			ReorderableListControl.DrawControlFromState(adaptor, null, flags);

			Rect bottomBoxRect = GUILayoutUtility.GetRect(sequence.nodeRect.width, 20);
			bottomBoxRect.y -= 7;
			bottomBoxRect.x += 5;
			bottomBoxRect.width -= 10;
			if (sequence.commandList.Count == 0)
			{
				bottomBoxRect.y -= 16;
			}

			GUIStyle boxStyle = new GUIStyle();
			boxStyle.border = new RectOffset(2, 2, 2, 1);
			boxStyle.margin = new RectOffset(5, 5, 5, 0);
			boxStyle.padding = new RectOffset(5, 5, 0, 0);
			boxStyle.alignment = TextAnchor.MiddleLeft;
			boxStyle.normal.background = FungusEditorResources.texTitleBackground;
			boxStyle.normal.textColor = EditorGUIUtility.isProSkin
										? new Color(0.8f, 0.8f, 0.8f)
										: new Color(0.2f, 0.2f, 0.2f);

			GUI.Box(bottomBoxRect, "", boxStyle);

			if (!Application.isPlaying &&
			    sequence == fungusScript.selectedSequence)
			{
				// Show add command button
				{
					Rect plusRect = bottomBoxRect;
					plusRect.x = plusRect.width - 19;
					plusRect.y += 2;
					plusRect.width = 16;
					plusRect.height = 16;

					if (GUI.Button(plusRect, FungusEditorResources.texAddButton, new GUIStyle()))
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

		void ShowCommandMenu()
		{
			Sequence sequence = target as Sequence;
			int index = sequence.commandList.Count;

			GenericMenu commandMenu = new GenericMenu();
			
			// Build menu list
			List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
			List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);

			foreach(var keyPair in filteredAttributes)
			{
				AddCommandOperation commandOperation = new AddCommandOperation();
				
				commandOperation.sequence = sequence;
				commandOperation.commandType = keyPair.Key;
				commandOperation.index = index;
				
				commandMenu.AddItem (new GUIContent (keyPair.Value.Category + "/" + keyPair.Value.CommandName), 
				                     false, AddCommandCallback, commandOperation);
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

	}

}