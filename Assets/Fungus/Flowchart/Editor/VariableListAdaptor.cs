// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System;
using Rotorz.ReorderableList;

namespace Fungus
{
	public class VariableListAdaptor : IReorderableListAdaptor {
		
		protected SerializedProperty _arrayProperty;

		public float fixedItemHeight;
		
		public SerializedProperty this[int index] {
			get { return _arrayProperty.GetArrayElementAtIndex(index); }
		}
		
		public SerializedProperty arrayProperty {
			get { return _arrayProperty; }
		}
		
		public VariableListAdaptor(SerializedProperty arrayProperty, float fixedItemHeight) {
			if (arrayProperty == null)
				throw new ArgumentNullException("Array property was null.");
			if (!arrayProperty.isArray)
				throw new InvalidOperationException("Specified serialized propery is not an array.");
			
			this._arrayProperty = arrayProperty;
			this.fixedItemHeight = fixedItemHeight;
		}
		
		public VariableListAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f) {
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
			// Remove the Fungus Variable component
			Variable variable = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as Variable;
			Undo.DestroyObjectImmediate(variable);

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

		public void BeginGUI()
		{}
		
		public void EndGUI()
		{}

		public virtual void DrawItemBackground(Rect position, int index) {
		}

		public void DrawItem(Rect position, int index) 
		{
			Variable variable = this[index].objectReferenceValue as Variable;

			if (variable == null)
			{
				return;
			}

			float[] widths = { 80, 100, 140, 60 };
			Rect[] rects = new Rect[4];

			for (int i = 0; i < 4; ++i)
			{
				rects[i] = position;
				rects[i].width = widths[i] - 5;

				for (int j = 0; j < i; ++j)
				{
					rects[i].x += widths[j];
				}
			}

			VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(variable.GetType());
			if (variableInfo == null)
			{
				return;
			}

			Flowchart flowchart = FlowchartWindow.GetFlowchart();
			if (flowchart == null)
			{
				return;
			}
							
			// Highlight if an active or selected command is referencing this variable
			bool highlight = false;
			if (flowchart.selectedBlock != null)
			{
				if (Application.isPlaying && flowchart.selectedBlock.IsExecuting())
				{
					highlight = flowchart.selectedBlock.activeCommand.HasReference(variable);
				}
				else if (!Application.isPlaying && flowchart.selectedCommands.Count > 0)
				{
					foreach (Command selectedCommand in flowchart.selectedCommands)
					{
						if (selectedCommand == null)
						{
							continue;
						}

						if (selectedCommand.HasReference(variable))
						{
							highlight = true;
							break;
						}
					}
				}
			}

			if (highlight)
			{
				GUI.backgroundColor = Color.green;
				GUI.Box(position, "");
			}

			string key = variable.key;
			VariableScope scope = variable.scope;

			// To access properties in a monobehavior, you have to new a SerializedObject
			// http://answers.unity3d.com/questions/629803/findrelativeproperty-never-worked-for-me-how-does.html
			SerializedObject variableObject = new SerializedObject(this[index].objectReferenceValue);

			variableObject.Update();

			GUI.Label(rects[0], variableInfo.VariableType);

			key = EditorGUI.TextField(rects[1], variable.key);
			SerializedProperty keyProp = variableObject.FindProperty("key");
			keyProp.stringValue = flowchart.GetUniqueVariableKey(key, variable);

			SerializedProperty defaultProp = variableObject.FindProperty("value");
			EditorGUI.PropertyField(rects[2], defaultProp, new GUIContent(""));

			SerializedProperty scopeProp = variableObject.FindProperty("scope");
			scope = (VariableScope)EditorGUI.EnumPopup(rects[3], variable.scope);
			scopeProp.enumValueIndex = (int)scope;

			variableObject.ApplyModifiedProperties();

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

