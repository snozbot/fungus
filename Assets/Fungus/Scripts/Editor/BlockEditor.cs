// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(Block))]
    public class BlockEditor : Editor
    {
        public static List<Action> actionList = new List<Action>();

        public static bool SelectedBlockDataStale { get; set; }

        protected Texture2D upIcon;
        protected Texture2D downIcon;
        protected Texture2D addIcon;
        protected Texture2D duplicateIcon;
        protected Texture2D deleteIcon;
        

        private CommandListAdaptor commandListAdaptor;
        private SerializedProperty commandListProperty;

        private Rect lastEventPopupPos, lastCMDpopupPos;

        private string callersString;
        private bool callersFoldout;

    
        protected virtual void OnEnable()
        {
            //this appears to happen when leaving playmode
            try
            {
                if (serializedObject == null)
                    return;
            }
            catch (Exception)
            {
                return;
            }

            upIcon = FungusEditorResources.Up;
            downIcon = FungusEditorResources.Down;
            addIcon = FungusEditorResources.Add;
            duplicateIcon = FungusEditorResources.Duplicate;
            deleteIcon = FungusEditorResources.Delete;

            commandListProperty = serializedObject.FindProperty("commandList");

            commandListAdaptor = new CommandListAdaptor(target as Block, commandListProperty);
        }

        protected void CacheCallerString()
        {
            if (!string.IsNullOrEmpty(callersString))
                return;

            var targetBlock = target as Block;

            var callers = FindObjectsOfType<MonoBehaviour>()
                .Where(x => x is IBlockCaller)
                .Select(x => x as IBlockCaller)
                .Where(x => x.MayCallBlock(targetBlock))
                .Select(x => x.GetLocationIdentifier()).ToArray();

            if (callers != null && callers.Length > 0)
                callersString = string.Join("\n", callers);
            else
                callersString = "None";

        }

        public virtual void DrawBlockName(Flowchart flowchart)
        {
            serializedObject.Update();

            SerializedProperty blockNameProperty = serializedObject.FindProperty("blockName");
            //calc position as size of what we want to draw pushed up into the top bar of the inspector
            //Rect blockLabelRect = new Rect(45, -GUI.skin.window.padding.bottom - EditorGUIUtility.singleLineHeight * 2, 120, 16);
            //EditorGUI.LabelField(blockLabelRect, new GUIContent("Block Name"));
            //Rect blockNameRect = new Rect(45, blockLabelRect.y + EditorGUIUtility.singleLineHeight, 180, 16);
            //EditorGUI.PropertyField(blockNameRect, blockNameProperty, new GUIContent(""));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Block Name"), EditorStyles.largeLabel);
            EditorGUI.BeginChangeCheck();
            blockNameProperty.stringValue = EditorGUILayout.TextField(blockNameProperty.stringValue);
            if(EditorGUI.EndChangeCheck())
            {
                // Ensure block name is unique for this Flowchart
                var block = target as Block;
                string uniqueName = flowchart.GetUniqueBlockKey(blockNameProperty.stringValue, block);
                if (uniqueName != block.BlockName)
                {
                    blockNameProperty.stringValue = uniqueName;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawBlockGUI(Flowchart flowchart)
        {
            serializedObject.Update();

            var block = target as Block;

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


            EditorGUI.BeginChangeCheck();

            if (block == flowchart.SelectedBlock)
            {
                // Custom tinting
                SerializedProperty useCustomTintProp = serializedObject.FindProperty("useCustomTint");
                SerializedProperty tintProp = serializedObject.FindProperty("tint");

                EditorGUILayout.BeginHorizontal();

                useCustomTintProp.boolValue = GUILayout.Toggle(useCustomTintProp.boolValue, " Custom Tint");
                if (useCustomTintProp.boolValue)
                {
                    EditorGUILayout.PropertyField(tintProp, GUIContent.none);
                }

                EditorGUILayout.EndHorizontal();

                SerializedProperty descriptionProp = serializedObject.FindProperty("description");
                EditorGUILayout.PropertyField(descriptionProp);


                SerializedProperty suppressProp = serializedObject.FindProperty("suppressAllAutoSelections");
                EditorGUILayout.PropertyField(suppressProp);
                
                EditorGUI.indentLevel++;
                if (callersFoldout = EditorGUILayout.Foldout(callersFoldout, "Callers"))
                {
                    CacheCallerString();
                    GUI.enabled = false;
                    EditorGUILayout.TextArea(callersString);
                    GUI.enabled = true;
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
                
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

                commandListAdaptor.DrawCommandList();

                // EventType.contextClick doesn't register since we moved the Block Editor to be inside
                // a GUI Area, no idea why. As a workaround we just check for right click instead.
                if (Event.current.type == EventType.MouseUp &&
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


            if (EditorGUI.EndChangeCheck())
            {
                SelectedBlockDataStale = true;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawButtonToolbar()
        {
            GUILayout.BeginHorizontal();


            // Previous Command
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.PageUp))
            {
                SelectPrevious();
                GUI.FocusControl("dummycontrol");
                Event.current.Use();
            }
            // Next Command
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.PageDown))
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


            //using false to prevent forcing a longer row than will fit on smallest inspector
            var pos = EditorGUILayout.GetControlRect(false, 0, EditorStyles.objectField);
            if (pos.x != 0)
            {
                lastCMDpopupPos = pos;
                lastCMDpopupPos.x += EditorGUIUtility.labelWidth;
                lastCMDpopupPos.y += EditorGUIUtility.singleLineHeight * 2;
            }
            // Add Button
            if (GUILayout.Button(addIcon))
            {
                //this may be less reliable for HDPI scaling but previous method using editor window height is now returning 
                //  null in 2019.2 suspect ongoing ui changes, so default to screen.height and then attempt to get the better result
                int h = Screen.height;
                if (EditorWindow.focusedWindow != null) h = (int)EditorWindow.focusedWindow.position.height;
                else if (EditorWindow.mouseOverWindow != null) h = (int)EditorWindow.mouseOverWindow.position.height;

                CommandSelectorPopupWindowContent.ShowCommandMenu(lastCMDpopupPos, "", target as Block,
                    (int)(EditorGUIUtility.currentViewWidth),
                    (int)(h - lastCMDpopupPos.y));
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

            var pos = EditorGUILayout.GetControlRect(true, 0, EditorStyles.objectField);
            if (pos.x != 0)
            {
                lastEventPopupPos = pos;
                lastEventPopupPos.x += EditorGUIUtility.labelWidth;
                lastEventPopupPos.y += EditorGUIUtility.singleLineHeight;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Execute On Event"));
            if (EditorGUILayout.DropdownButton(new GUIContent(currentHandlerName), FocusType.Passive))
            {
                EventSelectorPopupWindowContent.DoEventHandlerPopUp(lastEventPopupPos, currentHandlerName, block, (int)(EditorGUIUtility.currentViewWidth - lastEventPopupPos.x), 200);
            }
            EditorGUILayout.EndHorizontal();

            if (block._EventHandler != null)
            {
                EventHandlerEditor eventHandlerEditor = Editor.CreateEditor(block._EventHandler) as EventHandlerEditor;
                if (eventHandlerEditor != null)
                {
                    EditorGUI.BeginChangeCheck();
                    eventHandlerEditor.DrawInspectorGUI();

                    if(EditorGUI.EndChangeCheck())
                    {
                        SelectedBlockDataStale = true;
                    }

                    DestroyImmediate(eventHandlerEditor);
                }
            }
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
            blocks = blocks.OrderBy(x => x.BlockName).ToArray();

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
            blocks = blocks.OrderBy(x => x.BlockName).ToArray();

            for (int i = 0; i < blocks.Length; ++i)
            {
				blockNames.Add(new GUIContent(blocks[i].BlockName));

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
                commandMenu.AddItem(new GUIContent("Cut"), false, Cut);
            }
            else
            {
                commandMenu.AddDisabledItem(new GUIContent("Cut"));
            }

            if (showCopy)
            {
                commandMenu.AddItem(new GUIContent("Copy"), false, Copy);
            }
            else
            {
                commandMenu.AddDisabledItem(new GUIContent("Copy"));
            }

            if (showPaste)
            {
                commandMenu.AddItem(new GUIContent("Paste"), false, Paste);
            }
            else
            {
                commandMenu.AddDisabledItem(new GUIContent("Paste"));
            }

            if (showDelete)
            {
                commandMenu.AddItem(new GUIContent("Delete"), false, Delete);
            }
            else
            {
                commandMenu.AddDisabledItem(new GUIContent("Delete"));
            }

            if (showPlay)
            {
                commandMenu.AddItem(new GUIContent("Play from selected"), false, PlayCommand);
                commandMenu.AddItem(new GUIContent("Stop all and play"), false, StopAllPlayCommand);
            }

            commandMenu.AddSeparator("");

            commandMenu.AddItem(new GUIContent("Select All"), false, SelectAll);
            commandMenu.AddItem(new GUIContent("Select None"), false, SelectNone);

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
                flowchart.AddSelectedCommand(flowchart.SelectedBlock.CommandList[firstSelectedIndex - 1]);
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
            if (lastSelectedIndex < flowchart.SelectedBlock.CommandList.Count - 1)
            {
                flowchart.ClearSelectedCommands();
                flowchart.AddSelectedCommand(flowchart.SelectedBlock.CommandList[lastSelectedIndex + 1]);
            }

            Repaint();
        }



        public static List<KeyValuePair<System.Type, CommandInfoAttribute>> GetFilteredCommandInfoAttribute(List<System.Type> menuTypes)
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
            return filteredAttributes.Values.ToList<KeyValuePair<System.Type, CommandInfoAttribute>>();
        }

        // Compare delegate for sorting the list of command attributes
        public static int CompareCommandAttributes(KeyValuePair<System.Type, CommandInfoAttribute> x, KeyValuePair<System.Type, CommandInfoAttribute> y)
        {
            int compare = (x.Value.Category.CompareTo(y.Value.Category));
            if (compare == 0)
            {
                compare = (x.Value.CommandName.CompareTo(y.Value.CommandName));
            }
            return compare;
        }
    }
}
