// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System;
using Rotorz.ReorderableList;

namespace Fungus.EditorUtils
{
    public class CommandListAdaptor : IReorderableListAdaptor {
        
        protected SerializedProperty _arrayProperty;
        
        public float fixedItemHeight;
        
        public Rect nodeRect = new Rect();

        public SerializedProperty this[int index] {
            get { return _arrayProperty.GetArrayElementAtIndex(index); }
        }
        
        public SerializedProperty arrayProperty {
            get { return _arrayProperty; }
        }
        
        public CommandListAdaptor(SerializedProperty arrayProperty, float fixedItemHeight) {
            if (arrayProperty == null)
                throw new ArgumentNullException("Array property was null.");
            if (!arrayProperty.isArray)
                throw new InvalidOperationException("Specified serialized propery is not an array.");
            
            this._arrayProperty = arrayProperty;
            this.fixedItemHeight = fixedItemHeight;
        }
        
        public CommandListAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f) {
        }
        
        public int Count {
            get { return _arrayProperty.arraySize; }
        }
        
        public virtual bool CanDrag(int index) {
            return true;
        }
        
        public virtual bool CanRemove(int index) {
            return true;
        }
        
        public void Add() {
            Command newCommand = AddNewCommand();
            if (newCommand == null)
            {
                return;
            }
            
            int newIndex = _arrayProperty.arraySize;
            ++_arrayProperty.arraySize;
            _arrayProperty.GetArrayElementAtIndex(newIndex).objectReferenceValue = newCommand;
        }
        
        public void Insert(int index) {
            Command newCommand = AddNewCommand();
            if (newCommand == null)
            {
                return;
            }
            
            _arrayProperty.InsertArrayElementAtIndex(index);
            _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue = newCommand;
        }
        
        Command AddNewCommand()
        {
            Flowchart flowchart = FlowchartWindow.GetFlowchart();
            if (flowchart == null)
            {
                return null;
            }
            
            var block = flowchart.SelectedBlock;
            if (block == null)
            {
                return null;
            }
            
            var newCommand = Undo.AddComponent<Comment>(block.gameObject) as Command;
            newCommand.ItemId = flowchart.NextItemId();
            flowchart.ClearSelectedCommands();
            flowchart.AddSelectedCommand(newCommand);
            
            return newCommand;
        }
        
        public void Duplicate(int index) {
            
            Command command = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as Command;
            
            // Add the command as a new component
            var parentBlock = command.GetComponent<Block>();
            
            System.Type type = command.GetType();
            Command newCommand = Undo.AddComponent(parentBlock.gameObject, type) as Command;
            newCommand.ItemId = newCommand.GetFlowchart().NextItemId();
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(newCommand, field.GetValue(command));
            }
            
            _arrayProperty.InsertArrayElementAtIndex(index);
            _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue = newCommand;
        }
        
        public void Remove(int index) {
            // Remove the Fungus Command component
            Command command = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as Command;
            if (command != null)
            {
                Undo.DestroyObjectImmediate(command);
            }
            
            _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue = null;
            _arrayProperty.DeleteArrayElementAtIndex(index);
        }
        
        public void Move(int sourceIndex, int destIndex) {
            if (destIndex > sourceIndex)
                --destIndex;
            _arrayProperty.MoveArrayElement(sourceIndex, destIndex);
        }
        
        public void Clear() {
            while (Count > 0)
            {
                Remove(0);
            }
        }

        public void BeginGUI()
        {}

        public void EndGUI()
        {}

        public void DrawItemBackground(Rect position, int index) {
        }

        public void DrawItem(Rect position, int index) 
        {
            Command command = this[index].objectReferenceValue as Command;
            
            if (command == null)
            {
                return;
            }
            
            CommandInfoAttribute commandInfoAttr = CommandEditor.GetCommandInfo(command.GetType());
            if (commandInfoAttr == null)
            {
                return;
            }
            
            var flowchart = (Flowchart)command.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }
            
            bool isComment = command.GetType() == typeof(Comment);
            bool isLabel = (command.GetType() == typeof(Label));
            
            bool error = false;
            string summary = command.GetSummary();
            if (summary == null)
            {
                summary = "";
            }
            else
            {
                summary = summary.Replace("\n", "").Replace("\r", "");
            }
            if (summary.StartsWith("Error:"))
            {
                error = true;
            }

            if (isComment || isLabel)
            {
                summary = "<b> " + summary + "</b>";
            }
            else
            {
                summary = "<i>" + summary + "</i>";
            }

            bool commandIsSelected = false;
            foreach (Command selectedCommand in flowchart.SelectedCommands)
            {
                if (selectedCommand == command)
                {
                    commandIsSelected = true;
                    break;
                }
            }
            
            string commandName = commandInfoAttr.CommandName;
            
            GUIStyle commandLabelStyle = new GUIStyle(GUI.skin.box);
            commandLabelStyle.normal.background = FungusEditorResources.CommandBackground;
            int borderSize = 5;
            commandLabelStyle.border.top = borderSize;
            commandLabelStyle.border.bottom = borderSize;
            commandLabelStyle.border.left = borderSize;
            commandLabelStyle.border.right = borderSize;
            commandLabelStyle.alignment = TextAnchor.MiddleLeft;
            commandLabelStyle.richText = true;
            commandLabelStyle.fontSize = 11;
            commandLabelStyle.padding.top -= 1;
            
            float indentSize = 20;          
            for (int i = 0; i < command.IndentLevel; ++i)
            {
                Rect indentRect = position;
                indentRect.x += i * indentSize - 21;
                indentRect.width = indentSize + 1;
                indentRect.y -= 2;
                indentRect.height += 5;
                GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                GUI.Box(indentRect, "", commandLabelStyle);
            }
            
            float commandNameWidth = Mathf.Max(commandLabelStyle.CalcSize(new GUIContent(commandName)).x, 90f);
            float indentWidth = command.IndentLevel * indentSize;
            
            Rect commandLabelRect = position;
            commandLabelRect.x += indentWidth - 21;
            commandLabelRect.y -= 2;
            commandLabelRect.width -= (indentSize * command.IndentLevel - 22);
            commandLabelRect.height += 5;

            // There's a weird incompatibility between the Reorderable list control used for the command list and 
            // the UnityEvent list control used in some commands. In play mode, if you click on the reordering grabber
            // for a command in the list it causes the UnityEvent list to spew null exception errors.
            // The workaround for now is to hide the reordering grabber from mouse clicks by extending the command
            // selection rectangle to cover it. We are planning to totally replace the command list display system.
            Rect clickRect = position;
            clickRect.x -= 20;
            clickRect.width += 20;

            // Select command via left click
            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                clickRect.Contains(Event.current.mousePosition))
            {
                if (flowchart.SelectedCommands.Contains(command) && Event.current.button == 0)
                {
                    // Left click on already selected command
                    // Command key and shift key is not pressed
                    if (!EditorGUI.actionKey && !Event.current.shift)
                    {
                        BlockEditor.actionList.Add ( delegate {
                            flowchart.SelectedCommands.Remove(command);
                            flowchart.ClearSelectedCommands();
                        });
                    }

                    // Command key pressed
                    if (EditorGUI.actionKey)
                    {
                        BlockEditor.actionList.Add ( delegate {
                            flowchart.SelectedCommands.Remove(command);
                        });
                        Event.current.Use();
                    }
                }
                else
                {
                    bool shift = Event.current.shift;

                    // Left click and no command key
                    if (!shift && !EditorGUI.actionKey && Event.current.button == 0)
                    {
                        BlockEditor.actionList.Add ( delegate {
                            flowchart.ClearSelectedCommands();
                        });
                        Event.current.Use();
                    }

                    BlockEditor.actionList.Add ( delegate {
                        flowchart.AddSelectedCommand(command);
                    });

                    // Find first and last selected commands
                    int firstSelectedIndex = -1;
                    int lastSelectedIndex = -1;
                    if (flowchart.SelectedCommands.Count > 0)
                    { 
                        if ( flowchart.SelectedBlock != null)
                        {
                            for (int i = 0; i < flowchart.SelectedBlock.CommandList.Count; i++)
                            {
                                Command commandInBlock = flowchart.SelectedBlock.CommandList[i];                                
                                foreach (Command selectedCommand in flowchart.SelectedCommands)
                                {
                                    if (commandInBlock == selectedCommand)
                                    {
                                        firstSelectedIndex = i;
                                        break;
                                    }
                                }
                            }
                            for (int i = flowchart.SelectedBlock.CommandList.Count - 1; i >=0; i--)
                            {
                                Command commandInBlock = flowchart.SelectedBlock.CommandList[i];                                
                                foreach (Command selectedCommand in flowchart.SelectedCommands)
                                {
                                    if (commandInBlock == selectedCommand)
                                    {
                                        lastSelectedIndex = i;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (shift) 
                    {
                        int currentIndex = command.CommandIndex;
                        if (firstSelectedIndex == -1 ||
                            lastSelectedIndex == -1)
                        {
                            // No selected command found - select entire list
                            firstSelectedIndex = 0;
                            lastSelectedIndex = currentIndex;
                        }
                        else
                        {
                            if (currentIndex < firstSelectedIndex)
                            {
                                firstSelectedIndex = currentIndex;
                            }
                            if (currentIndex > lastSelectedIndex)
                            {
                                lastSelectedIndex = currentIndex;
                            }
                        }

                        for (int i = Math.Min(firstSelectedIndex, lastSelectedIndex); i < Math.Max(firstSelectedIndex, lastSelectedIndex); ++i)
                        {
                            var selectedCommand = flowchart.SelectedBlock.CommandList[i];
                            BlockEditor.actionList.Add ( delegate {
                                flowchart.AddSelectedCommand(selectedCommand);
                            });
                        }
                    }

                    Event.current.Use();
                }
                GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
            }
            
            Color commandLabelColor = Color.white;
            if (flowchart.ColorCommands)
            {
                commandLabelColor = command.GetButtonColor();
            }
            
            if (commandIsSelected)
            {
                commandLabelColor = Color.green;
            }
            else if (!command.enabled)
            {
                commandLabelColor = Color.grey;
            }
            else if (error)
            {
                // TODO: Show warning icon
            }
            
            GUI.backgroundColor = commandLabelColor;
            
            if (isComment)
            {
                GUI.Label(commandLabelRect, "", commandLabelStyle);
            }
            else
            {
                string commandNameLabel;
                if (flowchart.ShowLineNumbers)
                {
                    commandNameLabel = command.CommandIndex.ToString() + ": " + commandName;
                }
                else
                {
                    commandNameLabel = commandName;
                }

                GUI.Label(commandLabelRect, commandNameLabel, commandLabelStyle);
            }
            
            if (command.ExecutingIconTimer > Time.realtimeSinceStartup)
            {
                Rect iconRect = new Rect(commandLabelRect);
                iconRect.x += iconRect.width - commandLabelRect.width - 20;
                iconRect.width = 20;
                iconRect.height = 20;

                Color storeColor = GUI.color;

                float alpha = (command.ExecutingIconTimer - Time.realtimeSinceStartup) / FungusConstants.ExecutingIconFadeTime;
                alpha = Mathf.Clamp01(alpha);

                GUI.color = new Color(1f, 1f, 1f, alpha);
                GUI.Label(iconRect, FungusEditorResources.PlaySmall, new GUIStyle());

                GUI.color = storeColor;
            }
            
            Rect summaryRect = new Rect(commandLabelRect);
            if (isComment)
            {
                summaryRect.x += 5;
            }
            else
            {
                summaryRect.x += commandNameWidth + 5;
                summaryRect.width -= commandNameWidth + 5;
            }
            
            GUIStyle summaryStyle = new GUIStyle();
            summaryStyle.fontSize = 10; 
            summaryStyle.padding.top += 5;
            summaryStyle.richText = true;
            summaryStyle.wordWrap = false;
            summaryStyle.clipping = TextClipping.Clip;
            commandLabelStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(summaryRect, summary, summaryStyle);
            
            if (error)
            {
                GUISkin editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
                Rect errorRect = new Rect(summaryRect);
                errorRect.x += errorRect.width - 20;
                errorRect.y += 2;
                errorRect.width = 20;
                GUI.Label(errorRect, editorSkin.GetStyle("CN EntryError").normal.background);
                summaryRect.width -= 20;
            }
            
            GUI.backgroundColor = Color.white;
        }
        
        public virtual float GetItemHeight(int index) {
            return fixedItemHeight != 0f
                ? fixedItemHeight
                    : EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false)
                    ;
        }
        
        
    }
}
