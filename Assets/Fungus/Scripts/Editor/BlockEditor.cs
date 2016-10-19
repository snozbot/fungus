// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Rotorz.ReorderableList;
using System.IO;
using System.Reflection;

namespace Fungus.EditorUtils
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

        public static List<Action> actionList = new List<Action>();

        protected Texture2D upIcon;
        protected Texture2D downIcon;
        protected Texture2D addIcon;
        protected Texture2D duplicateIcon;
        protected Texture2D deleteIcon;

        protected virtual void OnEnable()
        {
            upIcon = Resources.Load("Icons/up") as Texture2D;
            downIcon = Resources.Load("Icons/down") as Texture2D;
            addIcon = Resources.Load("Icons/add") as Texture2D;
            duplicateIcon = Resources.Load("Icons/duplicate") as Texture2D;
            deleteIcon = Resources.Load("Icons/delete") as Texture2D;
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
            var block = target as Block;
            string uniqueName = flowchart.GetUniqueBlockKey(blockNameProperty.stringValue, block);
            if (uniqueName != block.BlockName)
            {
                blockNameProperty.stringValue = uniqueName;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawBlockGUI(Flowchart flowchart)
        {
            serializedObject.Update();

            // Execute any queued cut, copy, paste, etc. operations from the prevous GUI update
            // We need to defer applying these operations until the following update because
            // the ReorderableList control emits GUI errors if you clear the list in the same frame
            // as drawing the control (e.g. select all and then delete)
            if (Event.current.type == EventType.Layout)
            {
                foreach (Action action in actionList)
                {
                    if (action != null)
                    {
                        action();
                    }
                }
                actionList.Clear();
            }

            var block = target as Block;

            SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");
            
            if (block == flowchart.SelectedBlock)
            {
                SerializedProperty descriptionProp = serializedObject.FindProperty("description");
                EditorGUILayout.PropertyField(descriptionProp);

                DrawEventHandlerGUI(flowchart);
                
                block.UpdateIndentLevels();

                // Make sure each command has a reference to its parent block
                foreach (var command in block.CommandList)
                {
                    if (command == null) // Will be deleted from the list later on
                    {
                        continue;
                    }
                    command.ParentBlock = block;
                }

                ReorderableListGUI.Title("Commands");
                CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
                adaptor.nodeRect = block._NodeRect;
                
                ReorderableListFlags flags = ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.DisableContextMenu;

                if (block.CommandList.Count == 0)
                {
                    EditorGUILayout.HelpBox("Press the + button below to add a command to the list.", MessageType.Info);
                }
                else
                {
                    ReorderableListControl.DrawControlFromState(adaptor, null, flags);
                }

                // EventType.contextClick doesn't register since we moved the Block Editor to be inside
                // a GUI Area, no idea why. As a workaround we just check for right click instead.
                if (Event.current.type == EventType.mouseUp &&
                    Event.current.button == 1)
                {
                    ShowContextMenu();
                    Event.current.Use();
                }

                if (GUIUtility.keyboardControl == 0) //Only call keyboard shortcuts when not typing in a text field
                {
                    Event e = Event.current;
                    
                    // Copy keyboard shortcut
                    if (e.type == EventType.ValidateCommand && e.commandName == "Copy")
                    {
                        if (flowchart.SelectedCommands.Count > 0)
                        {
                            e.Use();
                        }
                    }

                    if (e.type == EventType.ExecuteCommand && e.commandName == "Copy")      
                    {
                        actionList.Add(Copy);
                        e.Use();
                    }
                    
                    // Cut keyboard shortcut
                    if (e.type == EventType.ValidateCommand && e.commandName == "Cut")
                    {
                        if (flowchart.SelectedCommands.Count > 0)
                        {
                            e.Use();
                        }
                    }

                    if (e.type == EventType.ExecuteCommand && e.commandName == "Cut")
                    {
                        actionList.Add(Cut);
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
                        actionList.Add(Paste);
                        e.Use();
                    }
                    
                    // Duplicate keyboard shortcut
                    if (e.type == EventType.ValidateCommand && e.commandName == "Duplicate")
                    {
                        if (flowchart.SelectedCommands.Count > 0)
                        {
                            e.Use();
                        }
                    }

                    if (e.type == EventType.ExecuteCommand && e.commandName == "Duplicate")     
                    {
                        actionList.Add(Copy);
                        actionList.Add(Paste);
                        e.Use();
                    }
                    
                    // Delete keyboard shortcut
                    if (e.type == EventType.ValidateCommand && e.commandName == "Delete")
                    {
                        if (flowchart.SelectedCommands.Count > 0)
                        {
                            e.Use();
                        }
                    }

                    if (e.type == EventType.ExecuteCommand && e.commandName == "Delete")        
                    {
                        actionList.Add(Delete);
                        e.Use();
                    }
                    
                    // SelectAll keyboard shortcut
                    if (e.type == EventType.ValidateCommand && e.commandName == "SelectAll")
                    {
                        e.Use();
                    }
                
                    if (e.type == EventType.ExecuteCommand && e.commandName == "SelectAll")     
                    {
                        actionList.Add(SelectAll);
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

            if (GUILayout.Button(upIcon))
            {
                SelectPrevious();
            }
            
            // Down Button
            if (GUILayout.Button(downIcon))
            {
                SelectNext();
            }
            
            GUILayout.FlexibleSpace();
            
            // Add Button
            if (GUILayout.Button(addIcon))
            {
                ShowCommandMenu();
            }
            
            // Duplicate Button
            if (GUILayout.Button(duplicateIcon))
            {
                Copy();
                Paste();
            }
            
            // Delete Button
            if (GUILayout.Button(deleteIcon))
            {
                Delete();
            }
            
            GUILayout.EndHorizontal();
        }

        protected virtual void DrawEventHandlerGUI(Flowchart flowchart)
        {
            // Show available Event Handlers in a drop down list with type of current
            // event handler selected.
            List<System.Type> eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).ToList();

            Block block = target as Block;
            System.Type currentType = null;
            if (block._EventHandler != null)
            {
                currentType = block._EventHandler.GetType();
            }

            string currentHandlerName = "<None>";
            if (currentType != null)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(currentType);
                if (info != null)
                {
                    currentHandlerName = info.EventHandlerName;
                }
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
                    if (info != null &&
                        info.Category.Length == 0)
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
                    if (info != null && 
                        info.Category.Length > 0)
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

            if (block._EventHandler != null)
            {
                EventHandlerEditor eventHandlerEditor = Editor.CreateEditor(block._EventHandler) as EventHandlerEditor;
                if (eventHandlerEditor != null)
                {
                    eventHandlerEditor.DrawInspectorGUI();
                    DestroyImmediate(eventHandlerEditor);
                }
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

            if (block._EventHandler != null)
            {
                Undo.DestroyObjectImmediate(block._EventHandler);
            }

            if (selectedType != null)
            {
                EventHandler newHandler = Undo.AddComponent(block.gameObject, selectedType) as EventHandler;
                newHandler.ParentBlock = block;
                block._EventHandler = newHandler;
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(block);
        }

        public static void BlockField(SerializedProperty property, GUIContent label, GUIContent nullLabel, Flowchart flowchart)
        {
            if (flowchart == null)
            {
                return;
            }

            var block = property.objectReferenceValue as Block;
        
            // Build dictionary of child blocks
            List<GUIContent> blockNames = new List<GUIContent>();
            
            int selectedIndex = 0;
            blockNames.Add(nullLabel);
            var blocks = flowchart.GetComponents<Block>();
            for (int i = 0; i < blocks.Length; ++i)
            {
                blockNames.Add(new GUIContent(blocks[i].BlockName));
                
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

        public static Block BlockField(Rect position, GUIContent nullLabel, Flowchart flowchart, Block block)
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
            Block[] blocks = flowchart.GetComponents<Block>();
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

        [MenuItem("Tools/Fungus/Utilities/Export Reference Docs")]
        protected static void ExportReferenceDocs()
        {
            const string path = "./Docs";

            ExportCommandInfo(path);
            ExportEventHandlerInfo(path);

            FlowchartWindow.ShowNotification("Exported Reference Documentation");
        }

        protected static void ExportCommandInfo(string path)
        {
            // Dump command info
            List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
            List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);
            filteredAttributes.Sort( CompareCommandAttributes );
            
            // Build list of command categories
            List<string> commandCategories = new List<string>();
            foreach(var keyPair in filteredAttributes)
            {
                CommandInfoAttribute info = keyPair.Value;
                if (info.Category != "" &&
                    !commandCategories.Contains(info.Category))
                {
                    commandCategories.Add (info.Category);
                }
            }
            commandCategories.Sort();
            
            // Output the commands in each category
            foreach (string category in commandCategories)
            {
                string markdown = "# " + category + " commands # {#" + category.ToLower() + "_commands}\n\n";
                markdown += "[TOC]\n";

                foreach(var keyPair in filteredAttributes)
                {
                    CommandInfoAttribute info = keyPair.Value;
                    
                    if (info.Category == category ||
                        info.Category == "" && category == "Scripting")
                    {
                        markdown += "# " + info.CommandName + " # {#" + info.CommandName.Replace(" ", "") + "}\n";
                        markdown += info.HelpText + "\n\n";
                        markdown += "Defined in " + keyPair.Key.FullName + "\n";
                        markdown += GetPropertyInfo(keyPair.Key);
                    }
                }
                
                string filePath = path + "/command_ref/" + category.ToLower() + "_commands.md";
                
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, markdown);
            }
        }

        protected static void ExportEventHandlerInfo(string path)
        {
            List<System.Type> eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).ToList();
            List<string> eventHandlerCategories = new List<string>();
            eventHandlerCategories.Add("Core");
            foreach (System.Type type in eventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null &&
                    info.Category != "" &&
                    !eventHandlerCategories.Contains(info.Category))
                {
                    eventHandlerCategories.Add(info.Category);
                }
            }
            eventHandlerCategories.Sort();
            
            // Output the commands in each category
            foreach (string category in eventHandlerCategories)
            {
                string markdown = "# " + category + " event handlers # {#" + category.ToLower() + "_events}\n\n";
                markdown += "[TOC]\n";

                foreach (System.Type type in eventHandlerTypes)
                {
                    EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);

                    if (info != null &&
                        info.Category == category ||
                        info.Category == "" && category == "Core")
                    {
                        markdown += "# " + info.EventHandlerName + " # {#" + info.EventHandlerName.Replace(" ", "") + "}\n";
                        markdown += info.HelpText + "\n\n";
                        markdown += "Defined in " + type.FullName + "\n";
                        markdown += GetPropertyInfo(type);
                    }
                }
                
                string filePath = path + "/command_ref/" + category.ToLower() + "_events.md";
                
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, markdown);
            }           
        }

        protected static string GetPropertyInfo(System.Type type)
        {
            string markdown = "";
            foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                TooltipAttribute attribute = (TooltipAttribute)Attribute.GetCustomAttribute(field, typeof(TooltipAttribute));
                if (attribute == null )
                {
                    continue;
                }

                // Change field name to how it's displayed in the inspector
                string propertyName = Regex.Replace(field.Name, "(\\B[A-Z])", " $1");
                if (propertyName.Length > 1)
                {
                    propertyName = propertyName.Substring(0,1).ToUpper() + propertyName.Substring(1);
                }
                else
                {
                    propertyName = propertyName.ToUpper();
                }

                markdown += propertyName + " | " + field.FieldType + " | " + attribute.tooltip + "\n";
            }

            if (markdown.Length > 0)
            {
                markdown = "\nProperty | Type | Description\n --- | --- | ---\n" + markdown + "\n";
            }

            return markdown;
        }

        protected virtual void ShowCommandMenu()
        {
            var block = target as Block;

            var flowchart = (Flowchart)block.GetFlowchart();

            // Use index of last selected command in list, or end of list if nothing selected.
            int index = -1;
            foreach (var command in flowchart.SelectedCommands)
            {
                if (command.CommandIndex + 1 > index)
                {
                    index = command.CommandIndex + 1;
                }
            }
            if (index == -1)
            {
                index = block.CommandList.Count;
            }

            GenericMenu commandMenu = new GenericMenu();
            
            // Build menu list
            List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
            List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);

            filteredAttributes.Sort( CompareCommandAttributes );

            foreach(var keyPair in filteredAttributes)
            {
                // Skip command type if the Flowchart doesn't support it
                if (!flowchart.IsCommandSupported(keyPair.Value))
                {
                    continue;
                }       

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
            
            var block = commandOperation.block;
            if (block == null)
            {
                return;
            }

            var flowchart = (Flowchart)block.GetFlowchart();

            flowchart.ClearSelectedCommands();
            
            var newCommand = Undo.AddComponent(block.gameObject, commandOperation.commandType) as Command;
            block.GetFlowchart().AddSelectedCommand(newCommand);
            newCommand.ParentBlock = block;
            newCommand.ItemId = flowchart.NextItemId();

            // Let command know it has just been added to the block
            newCommand.OnCommandAdded(block);

            Undo.RecordObject(block, "Set command type");
            if (commandOperation.index < block.CommandList.Count - 1)
            {
                block.CommandList.Insert(commandOperation.index, newCommand);
            }
            else
            {
                block.CommandList.Add(newCommand);
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(block);
        }

        public virtual void ShowContextMenu()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null)
            {
                return;
            }
            
            bool showCut = false;
            bool showCopy = false;
            bool showDelete = false;
            bool showPaste = false;
            bool showPlay = false;

            if (flowchart.SelectedCommands.Count > 0)
            {
                showCut = true;
                showCopy = true;
                showDelete = true;
                if (flowchart.SelectedCommands.Count == 1 && Application.isPlaying)
                {
                    showPlay = true;
                }
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

            if (showPlay)
            {
                commandMenu.AddItem(new GUIContent("Play from selected"), false, PlayCommand);
                commandMenu.AddItem(new GUIContent("Stop all and play"), false, StopAllPlayCommand);
            }

            commandMenu.AddSeparator("");
            
            commandMenu.AddItem (new GUIContent ("Select All"), false, SelectAll);
            commandMenu.AddItem (new GUIContent ("Select None"), false, SelectNone);

            commandMenu.ShowAsContext();
        }
        
        protected void SelectAll()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null ||
                flowchart.SelectedBlock == null)
            {
                return;
            }
            
            flowchart.ClearSelectedCommands();
            Undo.RecordObject(flowchart, "Select All");
            foreach (Command command in flowchart.SelectedBlock.CommandList)
            {
                flowchart.AddSelectedCommand(command);
            }

            Repaint();
        }
        
        protected void SelectNone()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null ||
                flowchart.SelectedBlock == null)
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
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null ||
                flowchart.SelectedBlock == null)
            {
                return;
            }
            
            CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
            commandCopyBuffer.Clear();

            // Scan through all commands in execution order to see if each needs to be copied
            foreach (Command command in flowchart.SelectedBlock.CommandList)
            {
                if (flowchart.SelectedCommands.Contains(command))
                {
                    var type = command.GetType();
                    Command newCommand = Undo.AddComponent(commandCopyBuffer.gameObject, type) as Command;
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    foreach (var field in fields)
                    {
                        // Copy all public fields
                        bool copy = field.IsPublic;

                        // Copy non-public fields that have the SerializeField attribute
                        var attributes = field.GetCustomAttributes(typeof(SerializeField), true);
                        if (attributes.Length > 0)
                        {
                            copy = true;
                        }

                        if (copy)
                        {
                            field.SetValue(newCommand, field.GetValue(command));
                        }
                    }
                }
            }
        }
        
        protected void Paste()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null ||
                flowchart.SelectedBlock == null)
            {
                return;
            }
            
            CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
            
            // Find where to paste commands in block (either at end or after last selected command)
            int pasteIndex = flowchart.SelectedBlock.CommandList.Count;
            if (flowchart.SelectedCommands.Count > 0)
            {
                for (int i = 0; i < flowchart.SelectedBlock.CommandList.Count; ++i)
                {
                    Command command = flowchart.SelectedBlock.CommandList[i];
                    
                    foreach (Command selectedCommand in flowchart.SelectedCommands)
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
                            pastedCommand.ItemId = flowchart.NextItemId();
                            flowchart.SelectedBlock.CommandList.Insert(pasteIndex++, pastedCommand);
                        }
                    }

                    // This stops the user pasting the command manually into another game object.
                    ComponentUtility.CopyComponent(flowchart.transform);
                }
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(block);
            
            Repaint();
        }
        
        protected void Delete()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null ||
                flowchart.SelectedBlock == null)
            {
                return;
            }
            int lastSelectedIndex = 0;
            for (int i = flowchart.SelectedBlock.CommandList.Count - 1; i >= 0; --i)
            {
                Command command = flowchart.SelectedBlock.CommandList[i];
                foreach (Command selectedCommand in flowchart.SelectedCommands)
                {
                    if (command == selectedCommand)
                    {
                        command.OnCommandRemoved(block);

                        // Order of destruction is important here for undo to work
                        Undo.DestroyObjectImmediate(command);

                        Undo.RecordObject((Block)flowchart.SelectedBlock, "Delete");
                        flowchart.SelectedBlock.CommandList.RemoveAt(i);

                        lastSelectedIndex = i;

                        break;
                    }
                }
            }

            Undo.RecordObject(flowchart, "Delete");
            flowchart.ClearSelectedCommands();

            if (lastSelectedIndex < flowchart.SelectedBlock.CommandList.Count)
            {
                var nextCommand = flowchart.SelectedBlock.CommandList[lastSelectedIndex];
                block.GetFlowchart().AddSelectedCommand(nextCommand);
            }

            Repaint();
        }
        
        protected void PlayCommand()
        {
            var targetBlock = target as Block;
            var flowchart = (Flowchart)targetBlock.GetFlowchart();
            Command command = flowchart.SelectedCommands[0];
            if (targetBlock.IsExecuting())
            {
                // The Block is already executing.
                // Tell the Block to stop, wait a little while so the executing command has a 
                // chance to stop, and then start execution again from the new command. 
                targetBlock.Stop();
                flowchart.StartCoroutine(RunBlock(flowchart, targetBlock, command.CommandIndex, 0.2f));
            }
            else
            {
                // Block isn't executing yet so can start it now.
                flowchart.ExecuteBlock(targetBlock, command.CommandIndex);
            }
        }

        protected void StopAllPlayCommand()
        {
            var targetBlock = target as Block;
            var flowchart = (Flowchart)targetBlock.GetFlowchart();
            Command command = flowchart.SelectedCommands[0];

            // Stop all active blocks then run the selected block.
            flowchart.StopAllBlocks();
            flowchart.StartCoroutine(RunBlock(flowchart, targetBlock, command.CommandIndex, 0.2f));
        }

        protected IEnumerator RunBlock(Flowchart flowchart, Block targetBlock, int commandIndex, float delay)
        {
            yield return new WaitForSeconds(delay);
            flowchart.ExecuteBlock(targetBlock, commandIndex);
        }

        protected void SelectPrevious()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();
            
            int firstSelectedIndex = flowchart.SelectedBlock.CommandList.Count;
            bool firstSelectedCommandFound = false;
            if (flowchart.SelectedCommands.Count > 0)
            {
                for (int i = 0; i < flowchart.SelectedBlock.CommandList.Count; i++)
                {
                    Command commandInBlock = flowchart.SelectedBlock.CommandList[i];
                    
                    foreach (Command selectedCommand in flowchart.SelectedCommands)
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
                flowchart.AddSelectedCommand(flowchart.SelectedBlock.CommandList[firstSelectedIndex-1]);
            }
            
            Repaint();
        }

        protected void SelectNext()
        {
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();
            
            int lastSelectedIndex = -1;
            if (flowchart.SelectedCommands.Count > 0)
            {
                for (int i = 0; i < flowchart.SelectedBlock.CommandList.Count; i++)
                {
                    Command commandInBlock = flowchart.SelectedBlock.CommandList[i];
                    
                    foreach (Command selectedCommand in flowchart.SelectedCommands)
                    {
                        if (commandInBlock == selectedCommand)
                        {
                            lastSelectedIndex = i;
                        }
                    }
                }
            }
            if (lastSelectedIndex < flowchart.SelectedBlock.CommandList.Count-1)
            {
                flowchart.ClearSelectedCommands();
                flowchart.AddSelectedCommand(flowchart.SelectedBlock.CommandList[lastSelectedIndex+1]);
            }
            
            Repaint();
        }
    }    
}