// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Rotorz.ReorderableList;

namespace Fungus
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
			FungusScript fungusScript = FungusScriptWindow.GetFungusScript();
			if (fungusScript == null)
			{
				return null;
			}
			
			Sequence sequence = fungusScript.selectedSequence;
			if (sequence == null)
			{
				return null;
			}

			Command newCommand = Undo.AddComponent<Comment>(sequence.gameObject) as Command;
			fungusScript.ClearSelectedCommands();
			fungusScript.AddSelectedCommand(newCommand);

			return newCommand;
		}

		public void Duplicate(int index) {

			Command command = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as Command;

			// Add the command as a new component
			Sequence parentSequence = command.GetComponent<Sequence>();

			System.Type type = command.GetType();
			Command newCommand = Undo.AddComponent(parentSequence.gameObject, type) as Command;
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

			FungusScript fungusScript = command.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			bool isComment = command.GetType() == typeof(Comment);

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

			if (summary.Length > 80)
			{
				summary = summary.Substring(0, 80) + "...";
			}
			if (summary.StartsWith("Error:"))
			{
				error = true;
			}
			summary = "<i>" + summary + "</i>";

			bool commandIsSelected = false;
			foreach (Command selectedCommand in fungusScript.selectedCommands)
			{
				if (selectedCommand == command)
				{
					commandIsSelected = true;
					break;
				}
			}

			string commandName = commandInfoAttr.CommandName;

			GUIStyle commandLabelStyle = new GUIStyle(GUI.skin.box);
			commandLabelStyle.normal.background = FungusEditorResources.texCommandBackground;
			commandLabelStyle.border.top = 1;
			commandLabelStyle.border.bottom = 1;
			commandLabelStyle.border.left = 1;
			commandLabelStyle.border.right = 1;
			commandLabelStyle.alignment = TextAnchor.MiddleLeft;
			commandLabelStyle.richText = true;
			commandLabelStyle.fontSize = 11;
			commandLabelStyle.padding.top -= 1;

			float indentSize = 20;			
			for (int i = 0; i < command.indentLevel; ++i)
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
			float indentWidth = command.indentLevel * indentSize;

			Rect commandLabelRect = position;
			commandLabelRect.x += indentWidth - 21;
			commandLabelRect.y -= 2;
			commandLabelRect.width -= (indentSize * command.indentLevel - 22);
			commandLabelRect.height += 5;

			// Select command via left click
			if (Event.current.type == EventType.MouseDown &&
			    Event.current.button == 0 &&
			    position.Contains(Event.current.mousePosition))
			{
				if (fungusScript.selectedCommands.Contains(command) && Event.current.button == 0)
				{
					// Left click on an already selected command
					fungusScript.selectedCommands.Remove(command);
				}
				else
				{
					// Left click and no command key
					if (!EditorGUI.actionKey && Event.current.button == 0)
					{
						fungusScript.ClearSelectedCommands();
					}

					fungusScript.AddSelectedCommand(command);
				}
				GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
			}

			Color commandLabelColor = Color.white;
			if (fungusScript.colorCommands)
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
				GUI.Label(commandLabelRect, commandName, commandLabelStyle);
			}

			if (command.IsExecuting())
			{
				Rect iconRect = new Rect(commandLabelRect);
				iconRect.x += iconRect.width - commandLabelRect.width - 20;
				iconRect.width = 20;
				iconRect.height = 20;
				GUI.Label(iconRect, FungusEditorResources.texPlaySmall, new GUIStyle());
			}

			Rect summaryRect = new Rect(commandLabelRect);
			if (!isComment)
			{
				summaryRect.x += commandNameWidth;
				summaryRect.width -= commandNameWidth;
				summaryRect.width -= 5;
			}

			GUIStyle summaryStyle = new GUIStyle();
			summaryStyle.fontSize = 10; 
			summaryStyle.padding.top += 5;
			summaryStyle.richText = true;
			summaryStyle.wordWrap = false;
			summaryStyle.clipping = TextClipping.Clip;
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
		
		private void ResetValue(SerializedProperty element) {
			switch (element.type) {
			case "string":
				element.stringValue = "";
				break;
			case "Vector2f":
				element.vector2Value = Vector2.zero;
				break;
			case "Vector3f":
				element.vector3Value = Vector3.zero;
				break;
			case "Rectf":
				element.rectValue = new Rect();
				break;
			case "Quaternionf":
				element.quaternionValue = Quaternion.identity;
				break;
			case "int":
				element.intValue = 0;
				break;
			case "float":
				element.floatValue = 0f;
				break;
			case "UInt8":
				element.boolValue = false;
				break;
			case "ColorRGBA":
				element.colorValue = Color.black;
				break;
				
			default:
				if (element.type.StartsWith("PPtr"))
					element.objectReferenceValue = null;
				break;
			}
		}
	}
}

