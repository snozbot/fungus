// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System;
using Rotorz.ReorderableList;

namespace Fungus.Script
{
	public class FungusCommandListAdaptor : IReorderableListAdaptor {
		
		private SerializedProperty _arrayProperty;

		public float fixedItemHeight;
		
		public SerializedProperty this[int index] {
			get { return _arrayProperty.GetArrayElementAtIndex(index); }
		}
		
		public SerializedProperty arrayProperty {
			get { return _arrayProperty; }
		}
		
		public FungusCommandListAdaptor(SerializedProperty arrayProperty, float fixedItemHeight) {
			if (arrayProperty == null)
				throw new ArgumentNullException("Array property was null.");
			if (!arrayProperty.isArray)
				throw new InvalidOperationException("Specified serialized propery is not an array.");
			
			this._arrayProperty = arrayProperty;
			this.fixedItemHeight = fixedItemHeight;
		}
		
		public FungusCommandListAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f) {
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
			int newIndex = _arrayProperty.arraySize;
			++_arrayProperty.arraySize;
			ResetValue(_arrayProperty.GetArrayElementAtIndex(newIndex));
		}

		public void Insert(int index) {
			_arrayProperty.InsertArrayElementAtIndex(index);
			ResetValue(_arrayProperty.GetArrayElementAtIndex(index));
		}

		public void Duplicate(int index) {
			_arrayProperty.InsertArrayElementAtIndex(index);
		}

		public void Remove(int index) {
			// Remove the Fungus Command component
			FungusCommand command = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as FungusCommand;
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
			_arrayProperty.ClearArray();
		}
		
		public void DrawItem(Rect position, int index) 
		{
			FungusCommand command = this[index].objectReferenceValue as FungusCommand;

			FungusScript fungusScript = command.GetFungusScript();
			
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
			
			if (!command.enabled)
			{
				GUI.backgroundColor = Color.grey;
			}
			else if (error)
			{
				GUI.backgroundColor = Color.red;
			}
			else if (fungusScript.selectedCommand == command)
			{
				GUI.backgroundColor = Color.yellow;
			}

			Rect buttonRect = position;
			buttonRect.width = 80;

			Rect summaryRect = position;
			summaryRect.x += 85;
			summaryRect.width -= 85;

			string commandName = FungusScriptEditor.GetCommandName(command.GetType());
			if (GUI.Button(buttonRect, commandName, EditorStyles.miniButton))
			{
				fungusScript.selectedCommand = command;
				GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
			}
			GUI.backgroundColor = Color.white;
			
			GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
			labelStyle.wordWrap = true;
			if (!command.enabled)
			{
				labelStyle.normal.textColor = Color.grey;
			}
			else if (error)
			{
				labelStyle.normal.textColor = Color.red;
			}
			
			GUI.Label(summaryRect, summary, labelStyle);

			if (Event.current.type == EventType.Repaint &&
			    command.IsExecuting())
			{
				GLDraw.DrawBox(position, Color.green, 1.5f);
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
	}
}

