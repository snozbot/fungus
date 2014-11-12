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
		
		public void DrawItem(Rect position, int index) 
		{
			Variable variable = this[index].objectReferenceValue as Variable;

			if (variable == null)
			{
				return;
			}

			float width1 = 100;
			float width3 = 50;
			float width2 = Mathf.Max(position.width - width1 - width3, 60);
			
			Rect keyRect = position;
			keyRect.width = width1;
			
			Rect valueRect = position;
			valueRect.x += width1 + 5;
			valueRect.width = width2 - 5;

			Rect scopeRect = position;
			scopeRect.x += width1 + width2 + 5;
			scopeRect.width = width3 - 5;
			
			string type = "";
			if (variable.GetType() == typeof(BooleanVariable))
			{
				type = "Boolean";
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				type = "Integer";
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				type = "Float";
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				type = "String";
			}

			FungusScript fungusScript = FungusScriptWindow.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}
							
			// Highlight if an active or selected command is referencing this variable
			bool highlight = false;
			if (fungusScript.selectedSequence != null)
			{
				if (Application.isPlaying && fungusScript.selectedSequence.IsExecuting())
				{
					highlight = fungusScript.selectedSequence.activeCommand.HasReference(variable);
				}
				else if (!Application.isPlaying && fungusScript.selectedCommands.Count > 0)
				{
					foreach (Command selectedCommand in fungusScript.selectedCommands)
					{
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

			if (Application.isPlaying)
			{
				GUI.Label(keyRect, variable.key);

				if (variable.GetType() == typeof(BooleanVariable))
				{
					BooleanVariable v = variable as BooleanVariable;
					v.Value = EditorGUI.Toggle(valueRect, v.Value);
				}
				else if (variable.GetType() == typeof(IntegerVariable))
				{
					IntegerVariable v = variable as IntegerVariable;
					v.Value = EditorGUI.IntField(valueRect, v.Value);
				}
				else if (variable.GetType() == typeof(FloatVariable))
				{
					FloatVariable v = variable as FloatVariable;
					v.Value = EditorGUI.FloatField(valueRect, v.Value);
				}
				else if (variable.GetType() == typeof(StringVariable))
				{
					StringVariable v = variable as StringVariable;
					v.Value = EditorGUI.TextField(valueRect, v.Value);
				}

				if (scope == VariableScope.Local)
				{
					GUI.Label(scopeRect, "Local");
				}
				else if (scope == VariableScope.Global)
				{
					GUI.Label(scopeRect, "Global");
				}
			}
			else
			{
				key = EditorGUI.TextField(keyRect, variable.key);
				GUI.Label(valueRect, type);
				scope = (VariableScope)EditorGUI.EnumPopup(scopeRect, variable.scope);

				// To access properties in a monobehavior, you have to new a SerializedObject
				// http://answers.unity3d.com/questions/629803/findrelativeproperty-never-worked-for-me-how-does.html
				SerializedObject variableObject = new SerializedObject(this[index].objectReferenceValue);
				SerializedProperty keyProp = variableObject.FindProperty("key");
				SerializedProperty scopeProp = variableObject.FindProperty("scope");

				variableObject.Update();
				keyProp.stringValue = fungusScript.GetUniqueVariableKey(key, variable);
				scopeProp.enumValueIndex = (int)scope;
				variableObject.ApplyModifiedProperties();
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

