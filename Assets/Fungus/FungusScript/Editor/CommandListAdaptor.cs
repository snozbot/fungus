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

			Command newCommand = Undo.AddComponent<Note>(sequence.gameObject) as Command;
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

			bool isNote = command.GetType() == typeof(Note);

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
			summary = "<i>" + summary + "</i>";

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

			GUIStyle commandLabelStyle = new GUIStyle(EditorStyles.miniButton);
			commandLabelStyle.alignment = TextAnchor.MiddleLeft;
			commandLabelStyle.richText = true;
			commandLabelStyle.fontSize = 11;
			commandLabelStyle.padding.top -= 1;

			float commandNameWidth = Mathf.Max(commandLabelStyle.CalcSize(new GUIContent(commandName)).x, 90f);
			float indentWidth = command.indentLevel * indentSize;

			Rect commandLabelRect = position;
			commandLabelRect.x += indentWidth;
			commandLabelRect.y -= 2;
			commandLabelRect.width -= (indentSize * command.indentLevel + 24);
			commandLabelRect.height += 6;

			if (!Application.isPlaying &&
			    Event.current.type == EventType.MouseDown &&
			    Event.current.button == 0 &&
			    position.Contains(Event.current.mousePosition))
			{
				fungusScript.selectedCommand = command;
				GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
			}

			Color commandLabelColor = Color.white;
			if (fungusScript.settings.colorCommands)
			{
				commandLabelColor = command.GetButtonColor();
			}

			if (highlight)
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

			if (!isNote)
			{
				GUI.backgroundColor = commandLabelColor;
				GUI.Label(commandLabelRect, commandName, commandLabelStyle);
			}

			Rect summaryRect = new Rect(commandLabelRect);
			if (!isNote)
			{
				summaryRect.x += commandNameWidth;
				summaryRect.width -= commandNameWidth;
			}

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

			GUIStyle summaryStyle = new GUIStyle(EditorStyles.miniLabel);
			summaryStyle.padding.top += 3;
			summaryStyle.richText = true;
			GUI.Label(summaryRect, summary, summaryStyle);

			GUI.backgroundColor = Color.white;

			if (!Application.isPlaying)
			{
				Rect menuRect = commandLabelRect;
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

			List<KeyValuePair<System.Type,CommandInfoAttribute>> filteredAttributes = GetFilteredCommandInfoAttribute(menuTypes);

			foreach(var keyPair in filteredAttributes)
			{
				SetCommandOperation commandOperation = new SetCommandOperation();
				
				commandOperation.sequence = sequence;
				commandOperation.commandType = keyPair.Key;
				commandOperation.index = index;
				
				commandMenu.AddItem (new GUIContent (keyPair.Value.Category + "/" + keyPair.Value.CommandName), 
				                     false, Callback, commandOperation);
			}

			commandMenu.ShowAsContext();
		}

		List<KeyValuePair<System.Type,CommandInfoAttribute>> GetFilteredCommandInfoAttribute(List<System.Type> menuTypes)
		{
			Dictionary<string, KeyValuePair<System.Type,CommandInfoAttribute>> filteredAttributes = new Dictionary<string, KeyValuePair<Type, CommandInfoAttribute>>();
			
			foreach(System.Type type in menuTypes)
			{
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object obj in attributes)
				{
					CommandInfoAttribute infoAttr = obj as CommandInfoAttribute;
					if (infoAttr != null)
					{
						string dictionnaryName = string.Format("{0}/{1}",infoAttr.Category,infoAttr.CommandName);
						
						int exisitingItemPriotiry = -1;
						if(filteredAttributes.ContainsKey(dictionnaryName))
							exisitingItemPriotiry = filteredAttributes[dictionnaryName].Value.Priority;
						
						if(infoAttr.Priority > exisitingItemPriotiry)
						{
							KeyValuePair<System.Type, CommandInfoAttribute> keyValuePair = new KeyValuePair<Type, CommandInfoAttribute>(type,infoAttr);
							filteredAttributes[dictionnaryName] = keyValuePair;
						}
					}
				}
			}
			return filteredAttributes.Values.ToList<KeyValuePair<System.Type,CommandInfoAttribute>>();
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

