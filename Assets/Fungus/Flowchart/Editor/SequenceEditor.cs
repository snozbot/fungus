using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rotorz.ReorderableList;
using System.IO;

namespace Fungus
{

	[CustomEditor (typeof(Block))]
	public class BlockEditor : Editor 
	{
		protected class SetEventHandlerOperation
		{
			public Block block;
			public Type eventHandlerType;
		}

		protected class AddCommandOperation
		{
			public Block block;
			public Type commandType;
			public int index;
		}

		public virtual void DrawBlockName(Flowchart flowchart)
		{
			serializedObject.Update();

			SerializedProperty blockNameProperty = serializedObject.FindProperty("blockName");
			Rect blockLabelRect = new Rect(45, 5, 120, 16);
			EditorGUI.LabelField(blockLabelRect, new GUIContent("Block Name"));
			Rect blockNameRect = new Rect(45, 21, 180, 16);
			EditorGUI.PropertyField(blockNameRect, blockNameProperty, new GUIContent(""));

			// Ensure block name is unique for this Flowchart
			Block block = target as Block;
			string uniqueName = flowchart.GetUniqueBlockKey(blockNameProperty.stringValue, block);
			if (uniqueName != block.blockName)
			{
				blockNameProperty.stringValue = uniqueName;
			}

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void DrawBlockGUI(Flowchart flowchart)
		{
			serializedObject.Update();

			Block block = target as Block;

			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");
			
			if (block == flowchart.selectedBlock)
			{
				SerializedProperty descriptionProp = serializedObject.FindProperty("description");
				EditorGUILayout.PropertyField(descriptionProp);
				
				SerializedProperty runSlowInEditorProp = serializedObject.FindProperty("runSlowInEditor");
				EditorGUILayout.PropertyField(runSlowInEditorProp);
				
				DrawEventHandlerGUI(flowchart);
				
				UpdateIndentLevels(block);

				// Make sure each command has a reference to its parent block
				foreach (Command command in block.commandList)
				{
					if (command == null) // Will be deleted from the list later on
					{
						continue;
					}
					command.parentBlock = block;
				}

				ReorderableListGUI.Title("Commands");
				CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
				adaptor.nodeRect = block.nodeRect;
				
				ReorderableListFlags flags = ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.DisableContextMenu;
				
				ReorderableListControl.DrawControlFromState(adaptor, null, flags);

				// EventType.contextClick doesn't register since we moved the Sequence Editor to be inside
				// a GUI Area, no idea why. As a workaround we just check for right click instead.
				if (Event.current.type == EventType.mouseUp &&
				    Event.current.button == 1)
				{
					ShowContextMenu();
				}

				if (GUIUtility.keyboardControl == 0) //Only call keyboard shortcuts when not typing in a text field
				{
					Event e = Event.current;
					
					// Copy keyboard shortcut
					if (e.type == EventType.ValidateCommand && e.commandName == "Copy")
					{
						if (flowchart.selectedCommands.Count > 0)
						{
							e.Use();
						}
					}

					if (e.type == EventType.ExecuteCommand && e.commandName == "Copy")		
					{
						Copy();
						e.Use();
					}
					
					// Cut keyboard shortcut
					if (e.type == EventType.ValidateCommand && e.commandName == "Cut")
					{
						if (flowchart.selectedCommands.Count > 0)
						{
							e.Use();
						}
					}

					if (e.type == EventType.ExecuteCommand && e.commandName == "Cut")
					{
						Cut();
						e.Use();
					}
					
					// Paste keyboard shortcut
					if (e.type == EventType.ValidateCommand && e.commandName == "Paste")
					{
						CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
						if (commandCopyBuffer.HasCommands())
						{
							e.Use();
						}
					}

					if (e.type == EventType.ExecuteCommand && e.commandName == "Paste")		
					{
						Paste();
						e.Use();
					}
					
					// Duplicate keyboard shortcut
					if (e.type == EventType.ValidateCommand && e.commandName == "Duplicate")
					{
						if (flowchart.selectedCommands.Count > 0)
						{
							e.Use();
						}
					}

					if (e.type == EventType.ExecuteCommand && e.commandName == "Duplicate")		
					{
						Copy();
						Paste();
						e.Use();
					}
					
					// Delete keyboard shortcut
					if (e.type == EventType.ValidateCommand && e.commandName == "Delete")
					{
						if (flowchart.selectedCommands.Count > 0)
						{
							e.Use();
						}
					}

					if (e.type == EventType.ExecuteCommand && e.commandName == "Delete")		
					{
						Delete();
						e.Use();
					}
					
					// SelectAll keyboard shortcut
					if (e.type == EventType.ValidateCommand && e.commandName == "SelectAll")
					{
						e.Use();
					}
				
					if (e.type == EventType.ExecuteCommand && e.commandName == "SelectAll")		
					{
						SelectAll();
						e.Use();
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
		}

		public virtual void DrawButtonToolbar()
		{
			GUILayout.BeginHorizontal();
			
			// Previous Command
			if ((Event.current.type == EventType.keyDown) && (Event.current.keyCode == KeyCode.PageUp))
			{
				SelectPrevious();
				GUI.FocusControl("dummycontrol");
				Event.current.Use();
			}
			// Next Command
			if ((Event.current.type == EventType.keyDown) && (Event.current.keyCode == KeyCode.PageDown))
			{
				SelectNext();
				GUI.FocusControl("dummycontrol");
				Event.current.Use();
			}
			
			// Up Button
			Texture2D upIcon = Resources.Load("Icons/up") as Texture2D;
			if (GUILayout.Button(upIcon))
			{
				SelectPrevious();
			}
			
			// Down Button
			Texture2D downIcon = Resources.Load("Icons/down") as Texture2D;
			if (GUILayout.Button(downIcon))
			{
				SelectNext();
			}
			
			GUILayout.FlexibleSpace();
			
			// Add Button
			Texture2D addIcon = Resources.Load("Icons/add") as Texture2D;
			if (GUILayout.Button(addIcon))
			{
				ShowCommandMenu();
			}
			
			// Duplicate Button
			Texture2D duplicateIcon = Resources.Load("Icons/duplicate") as Texture2D;
			if (GUILayout.Button(duplicateIcon))
			{
				Copy();
				Paste();
			}
			
			// Delete Button
			Texture2D deleteIcon = Resources.Load("Icons/delete") as Texture2D;
			if (GUILayout.Button(deleteIcon))
			{
				Delete();
			}
			
			GUILayout.EndHorizontal();
		}

		protected void DrawEventHandlerGUI(Flowchart flowchart)
		{
			// Show available Event Handlers in a drop down list with type of current
			// event handler selected.
			List<System.Type> eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).ToList();

			Block block = target as Block;
			System.Type currentType = null;
			if (block.eventHandler != null)
			{
				currentType = block.eventHandler.GetType();
			}

			string currentHandlerName = "<None>";
			if (currentType != null)
			{
				EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(currentType);
				currentHandlerName = info.EventHandlerName;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent("Execute On Event"));
			if (GUILayout.Button(new GUIContent(currentHandlerName), EditorStyles.popup))
			{
				SetEventHandlerOperation noneOperation = new SetEventHandlerOperation();
				noneOperation.block = block;
				noneOperation.eventHandlerType = null;
				
				GenericMenu eventHandlerMenu = new GenericMenu();
				eventHandlerMenu.AddItem(new GUIContent("None"), false, OnSelectEventHandler, noneOperation);

				// Add event handlers with no category first
				foreach (System.Type type in eventHandlerTypes)
				{
					EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);					
					if (info.Category.Length == 0)
					{
						SetEventHandlerOperation operation = new SetEventHandlerOperation();
						operation.block = block;
						operation.eventHandlerType = type;
						
						eventHandlerMenu.AddItem(new GUIContent(info.EventHandlerName), false, OnSelectEventHandler, operation);
					}
				}

				// Add event handlers with a category afterwards
				foreach (System.Type type in eventHandlerTypes)
				{
					EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);					
					if (info.Category.Length > 0)
					{			
						SetEventHandlerOperation operation = new SetEventHandlerOperation();
						operation.block = block;
						operation.eventHandlerType = type;
						string typeName = info.Category + "/" + info.EventHandlerName;
						eventHandlerMenu.AddItem(new GUIContent(typeName), false, OnSelectEventHandler, operation);
					}
				}


				eventHandlerMenu.ShowAsContext();
			}
			EditorGUILayout.EndHorizontal();

			if (block.eventHandler != null)
			{
				EventHandlerEditor eventHandlerEditor = Editor.CreateEditor(block.eventHandler) as EventHandlerEditor;
				eventHandlerEditor.DrawInspectorGUI();
				DestroyImmediate(eventHandlerEditor);
			}
		}

		protected void OnSelectEventHandler(object obj)
		{
			SetEventHandlerOperation operation = obj as SetEventHandlerOperation;
			Block block = operation.block;
			System.Type selectedType = operation.eventHandlerType;
			if (block == null)
			{
				return;
			}

			Undo.RecordObject(block, "Set Event Handler");

			if (block.eventHandler != null)
			{
				Undo.DestroyObjectImmediate(block.eventHandler);
			}

			if (selectedType != null)
			{
				EventHandler newHandler = Undo.AddComponent(block.gameObject, selectedType) as EventHandler;
				newHandler.parentBlock = block;
				block.eventHandler = newHandler;
			}
		}

		protected virtual void UpdateIndentLevels(Block block)
		{
			int indentLevel = 0;
			foreach(Command command in block.commandList)
			{
				if (command == null)
				{
					continue;
				}

				if (command.CloseBlock())
				{
					indentLevel--;
				}
				command.indentLevel = Math.Max(indentLevel, 0);

				if (command.OpenBlock())
				{
					indentLevel++;
				}
			}
		}

		static public void BlockField(SerializedProperty property, GUIContent label, GUIContent nullLabel, Flowchart flowchart)
		{
			if (flowchart == null)
			{
				return;
			}

			Block block = property.objectReferenceValue as Block;
		
			// Build dictionary of child blocks
			List<GUIContent> blockNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			blockNames.Add(nullLabel);
			Block[] blocks = flowchart.GetComponentsInChildren<Block>(true);
			for (int i = 0; i < blocks.Length; ++i)
			{
				blockNames.Add(new GUIContent(blocks[i].blockName));
				
				if (block == blocks[i])
				{
					selectedIndex = i + 1;
				}
			}
			
			selectedIndex = EditorGUILayout.Popup(label, selectedIndex, blockNames.ToArray());
			if (selectedIndex == 0)
			{
				block = null; // Option 'None'
			}
			else
			{
				block = blocks[selectedIndex - 1];
			}
			
			property.objectReferenceValue = block;
		}

		static public Block BlockField(Rect position, GUIContent nullLabel, Flowchart flowchart, Block block)
		{
			if (flowchart == null)
			{
				return null;
			}
			
			Block result = block;
			
			// Build dictionary of child blocks
			List<GUIContent> blockNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			blockNames.Add(nullLabel);
			Block[] blocks = flowchart.GetComponentsInChildren<Block>();
			for (int i = 0; i < blocks.Length; ++i)
			{
				blockNames.Add(new GUIContent(blocks[i].name));
				
				if (block == blocks[i])
				{
					selectedIndex = i + 1;
				}
			}
			
			selectedIndex = EditorGUI.Popup(position, selectedIndex, blockNames.ToArray());
			if (selectedIndex == 0)
			{
				result = null; // Option 'None'
			}
			else
			{
				result = blocks[selectedIndex - 1];
			}
			
			return result;
		}

		// Compare delegate for sorting the list of command attributes
		protected static int CompareCommandAttributes(KeyValuePair<System.Type, CommandInfoAttribute> x, KeyValuePair<System.Type, CommandInfoAttribute> y)
		{
			int compare = (x.Value.Category.CompareTo(y.Value.Category));
			if (compare == 0)
			{
				compare = (x.Value.CommandName.CompareTo(y.Value.CommandName));
			}
			return compare;
		}

		[MenuItem("Tools/Fungus/Utilities/Export Class Info")]
		protected static void DumpFungusClassInfo()
		{
			string path = EditorUtility.SaveFilePanel("Export strings", "",
			                                          "FungusClasses.csv", "");
			
			if(path.Length == 0) 
			{
				return;
			}

			string classInfo = "";

			// Dump command info
			List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
			List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);
			filteredAttributes.Sort( CompareCommandAttributes );
			foreach(var keyPair in filteredAttributes)
			{
				CommandInfoAttribute info = keyPair.Value;
				classInfo += ("Command," + info.CommandName + "," + info.Category + ",\"" + info.HelpText + "\"\n");
			}

			// Dump event handler info
			List<System.Type> eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).ToList();
			foreach (System.Type type in eventHandlerTypes)
			{
				EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
				classInfo += ("EventHandler," + info.EventHandlerName + "," + info.Category + ",\"" + info.HelpText + "\"\n");
			}

			File.WriteAllText(path, classInfo);
		}

		void ShowCommandMenu()
		{
			Block block = target as Block;

			Flowchart flowchart = block.GetFlowchart();

			// Use index of last selected command in list, or end of list if nothing selected.
			int index = -1;
			foreach (Command command in flowchart.selectedCommands)
			{
				if (command.commandIndex + 1 > index)
				{
					index = command.commandIndex + 1;
				}
			}
			if (index == -1)
			{
				index = block.commandList.Count;
			}

			GenericMenu commandMenu = new GenericMenu();
			
			// Build menu list
			List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
			List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);

			filteredAttributes.Sort( CompareCommandAttributes );

			foreach(var keyPair in filteredAttributes)
			{
				AddCommandOperation commandOperation = new AddCommandOperation();
				
				commandOperation.block = block;
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
		
		protected static List<KeyValuePair<System.Type,CommandInfoAttribute>> GetFilteredCommandInfoAttribute(List<System.Type> menuTypes)
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
		
		protected static void AddCommandCallback(object obj)
		{
			AddCommandOperation commandOperation = obj as AddCommandOperation;
			
			Block block = commandOperation.block;
			if (block == null)
			{
				return;
			}

			Flowchart flowchart = block.GetFlowchart();

			flowchart.ClearSelectedCommands();
			
			Command newCommand = Undo.AddComponent(block.gameObject, commandOperation.commandType) as Command;
			block.GetFlowchart().AddSelectedCommand(newCommand);
			newCommand.parentBlock = block;
			newCommand.commandId = flowchart.NextCommandId();

			// Let command know it has just been added to the block
			newCommand.OnCommandAdded(block);

			Undo.RecordObject(block, "Set command type");
			if (commandOperation.index < block.commandList.Count - 1)
			{
				block.commandList.Insert(commandOperation.index, newCommand);
			}
			else
			{
				block.commandList.Add(newCommand);
			}
		}

		public virtual void ShowContextMenu()
		{
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();

			if (flowchart == null)
			{
				return;
			}
			
			bool showCut = false;
			bool showCopy = false;
			bool showDelete = false;
			bool showPaste = false;
			
			if (flowchart.selectedCommands.Count > 0)
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
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();

			if (flowchart == null ||
			    flowchart.selectedBlock == null)
			{
				return;
			}
			
			flowchart.ClearSelectedCommands();
			Undo.RecordObject(flowchart, "Select All");
			foreach (Command command in flowchart.selectedBlock.commandList)
			{
				flowchart.AddSelectedCommand(command);
			}

			Repaint();
		}
		
		protected void SelectNone()
		{
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();

			if (flowchart == null ||
			    flowchart.selectedBlock == null)
			{
				return;
			}
			
			Undo.RecordObject(flowchart, "Select None");
			flowchart.ClearSelectedCommands();

			Repaint();
		}
		
		protected void Cut()
		{
			Copy();
			Delete();
		}
		
		protected void Copy()
		{
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();

			if (flowchart == null ||
			    flowchart.selectedBlock == null)
			{
				return;
			}
			
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			commandCopyBuffer.Clear();

			// Scan through all commands in execution order to see if each needs to be copied
			foreach (Command command in flowchart.selectedBlock.commandList)
			{
				if (flowchart.selectedCommands.Contains(command))
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
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();

			if (flowchart == null ||
			    flowchart.selectedBlock == null)
			{
				return;
			}
			
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			
			// Find where to paste commands in block (either at end or after last selected command)
			int pasteIndex = flowchart.selectedBlock.commandList.Count;
			if (flowchart.selectedCommands.Count > 0)
			{
				for (int i = 0; i < flowchart.selectedBlock.commandList.Count; ++i)
				{
					Command command = flowchart.selectedBlock.commandList[i];
					
					foreach (Command selectedCommand in flowchart.selectedCommands)
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
				// Using the Editor copy / paste functionality instead instead of reflection
				// because this does a deep copy of the command properties.
				if (ComponentUtility.CopyComponent(command))
				{
					if (ComponentUtility.PasteComponentAsNew(flowchart.gameObject))
					{
						Command[] commands = flowchart.GetComponents<Command>();
						Command pastedCommand = commands.Last<Command>();
						if (pastedCommand != null)
						{
							pastedCommand.commandId = flowchart.NextCommandId();
							flowchart.selectedBlock.commandList.Insert(pasteIndex++, pastedCommand);
						}
					}

					// This stops the user pasting the command manually into another game object.
					ComponentUtility.CopyComponent(flowchart.transform);
				}
			}

			Repaint();
		}
		
		protected void Delete()
		{
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();

			if (flowchart == null ||
			    flowchart.selectedBlock == null)
			{
				return;
			}
			int lastSelectedIndex = 0;
			for (int i = flowchart.selectedBlock.commandList.Count - 1; i >= 0; --i)
			{
				Command command = flowchart.selectedBlock.commandList[i];
				foreach (Command selectedCommand in flowchart.selectedCommands)
				{
					if (command == selectedCommand)
					{
						command.OnCommandRemoved(block);
						
						Undo.RecordObject(flowchart.selectedBlock, "Delete");
						flowchart.selectedBlock.commandList.RemoveAt(i);
						Undo.DestroyObjectImmediate(command);
						lastSelectedIndex = i;
						break;
					}
				}
			}
			
			Undo.RecordObject(flowchart, "Delete");
			flowchart.ClearSelectedCommands();
			
			if (lastSelectedIndex < flowchart.selectedBlock.commandList.Count)
			{
				Command nextCommand = flowchart.selectedBlock.commandList[lastSelectedIndex];
				block.GetFlowchart().AddSelectedCommand(nextCommand);
			}
			
			Repaint();
		}
		
		protected void SelectPrevious()
		{
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();
			
			int firstSelectedIndex = flowchart.selectedBlock.commandList.Count;
			bool firstSelectedCommandFound = false;
			if (flowchart.selectedCommands.Count > 0)
			{
				for (int i = 0; i < flowchart.selectedBlock.commandList.Count; i++)
				{
					Command commandInBlock = flowchart.selectedBlock.commandList[i];
					
					foreach (Command selectedCommand in flowchart.selectedCommands)
					{
						if (commandInBlock == selectedCommand)
						{
							if (!firstSelectedCommandFound)
							{
								firstSelectedIndex = i;
								firstSelectedCommandFound = true;
								break;
							}
						}
					}
					if (firstSelectedCommandFound)
					{
						break;
					}
				}
			}
			if (firstSelectedIndex > 0)
			{
				flowchart.ClearSelectedCommands();
				flowchart.AddSelectedCommand(flowchart.selectedBlock.commandList[firstSelectedIndex-1]);
			}
			
			Repaint();
		}

		protected void SelectNext()
		{
			Block block = target as Block;
			Flowchart flowchart = block.GetFlowchart();
			
			int lastSelectedIndex = -1;
			if (flowchart.selectedCommands.Count > 0)
			{
				for (int i = 0; i < flowchart.selectedBlock.commandList.Count; i++)
				{
					Command commandInBlock = flowchart.selectedBlock.commandList[i];
					
					foreach (Command selectedCommand in flowchart.selectedCommands)
					{
						if (commandInBlock == selectedCommand)
						{
							lastSelectedIndex = i;
						}
					}
				}
			}
			if (lastSelectedIndex < flowchart.selectedBlock.commandList.Count-1)
			{
				flowchart.ClearSelectedCommands();
				flowchart.AddSelectedCommand(flowchart.selectedBlock.commandList[lastSelectedIndex+1]);
			}
			
			Repaint();
		}
	}
	
}