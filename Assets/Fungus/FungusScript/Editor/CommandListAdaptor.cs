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

		protected class SetCommandOperation
		{
			public Sequence sequence;
			public Type commandType;
			public int index;
		}

		protected SerializedProperty _arrayProperty;

		public float fixedItemHeight;
		
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

			Command newCommand = sequence.gameObject.AddComponent<Note>() as Command;
			fungusScript.selectedCommand = newCommand;

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
			Undo.DestroyObjectImmediate(command);

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

			bool error = false;
			string summary = command.GetSummary().Replace("\n", "").Replace("\r", "");
			if (summary.Length > 80)
			{
				summary = summary.Substring(0, 80) + "...";
			}
			if (summary.StartsWith("Error:"))
			{
				error = true;
			}

			bool highlight = (Application.isPlaying && command.IsExecuting()) ||
							 (!Application.isPlaying && fungusScript.selectedCommand == command);

			float indentSize = 20;			
			for (int i = 0; i < command.indentLevel; ++i)
			{
				Rect indentRect = position;
				indentRect.x += i * indentSize;
				indentRect.width = indentSize + 1;
				indentRect.y -= 2;
				indentRect.height += 5;
				GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
				GUI.Box(indentRect, "");
			}

			string commandName = commandInfoAttr.CommandName;

			GUIStyle commandLabelStyle = new GUIStyle(EditorStyles.miniButtonLeft);

			float buttonWidth = Mathf.Max(commandLabelStyle.CalcSize(new GUIContent(commandName)).x, 100f);
			float indentWidth = command.indentLevel * indentSize;

			Rect buttonRect = position;
			buttonRect.x += indentWidth;
			buttonRect.width = buttonWidth;
			buttonRect.y -= 2;
			buttonRect.height += 6;

			Rect summaryRect = buttonRect;
			summaryRect.x += buttonWidth - 1;
			summaryRect.width = position.width - buttonWidth - indentWidth - 23;

			if (!Application.isPlaying &&
			    Event.current.type == EventType.MouseDown &&
			    Event.current.button == 0 &&
			    position.Contains(Event.current.mousePosition))
			{
				fungusScript.selectedCommand = command;
				GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
			}

			Color buttonBackgroundColor = Color.white;
			if (fungusScript.settings.colorCommands)
			{
				buttonBackgroundColor = command.GetButtonColor();
			}
			Color summaryBackgroundColor = Color.white;

			if (highlight)
			{
				summaryBackgroundColor = Color.green;
				buttonBackgroundColor = Color.green;
			}
			else if (!command.enabled)
			{
				buttonBackgroundColor = Color.grey;
				summaryBackgroundColor = Color.grey;
			}
			else if (error)
			{
				summaryBackgroundColor = Color.red;
			}

			GUI.backgroundColor = buttonBackgroundColor;
			GUI.Label(buttonRect, commandName, commandLabelStyle);

			GUIStyle summaryStyle = new GUIStyle(EditorStyles.miniButtonRight);
			summaryStyle.alignment = TextAnchor.MiddleLeft;
			if (error && !highlight)
			{
				summaryStyle.normal.textColor = Color.white;
			}

			GUI.backgroundColor = summaryBackgroundColor;
			GUI.Box(summaryRect, summary, summaryStyle);
		
			GUI.backgroundColor = Color.white;

			if (!Application.isPlaying)
			{
				Rect menuRect = summaryRect;
				menuRect.x += menuRect.width + 4;
				menuRect.y = position.y + 1;
				menuRect.width = 22;
				menuRect.height = position.height;
				GUIStyle menuButtonStyle = new GUIStyle("Foldout");
				if (GUI.Button(menuRect, new GUIContent("", "Select command type"), menuButtonStyle))
				{
					ShowCommandMenu(index, fungusScript.selectedSequence);
				}

				Rect selectRect = position;
				selectRect.x -= 19;
				selectRect.width = 20;
				command.selected = EditorGUI.Toggle(selectRect, command.selected);
			}
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

		void ShowCommandMenu(int index, Sequence sequence)
		{
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
						SetCommandOperation commandOperation = new SetCommandOperation();

						commandOperation.sequence = sequence;
						commandOperation.commandType = type;
						commandOperation.index = index;

						commandMenu.AddItem (new GUIContent (infoAttr.Category + "/" + infoAttr.CommandName), 
						                     false, Callback, commandOperation);
					}
				}
			}

			commandMenu.ShowAsContext();
		}

		void Callback(object obj)
		{
			SetCommandOperation commandOperation = obj as SetCommandOperation;

			Sequence sequence = commandOperation.sequence;
			if (sequence == null)
			{
				return;
			}

			Command newCommand = Undo.AddComponent(sequence.gameObject, commandOperation.commandType)  as Command;
			sequence.GetFungusScript().selectedCommand = newCommand;

			Command oldCommand = sequence.commandList[commandOperation.index];
			Undo.DestroyObjectImmediate(oldCommand);

			Undo.RecordObject(sequence, "Set command type");
			sequence.commandList[commandOperation.index] = newCommand;
		}
	}
}

