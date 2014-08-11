// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Rotorz.ReorderableList;

namespace Fungus.Script
{
	
	public class FungusVariableListAdaptor : IReorderableListAdaptor 
	{
		private SerializedProperty _arrayProperty;
		
		public float fixedItemHeight;
		
		public SerializedProperty this[int index] 
		{
			get { return _arrayProperty.GetArrayElementAtIndex(index); }
		}
		
		public SerializedProperty arrayProperty 
		{
			get { return _arrayProperty; }
		}
		
		public FungusVariableListAdaptor(SerializedProperty arrayProperty, float fixedItemHeight) 
		{
			if (arrayProperty == null)
				throw new ArgumentNullException("Array property was null.");
			if (!arrayProperty.isArray)
				throw new InvalidOperationException("Specified serialized propery is not an array.");
			
			this._arrayProperty = arrayProperty;
			this.fixedItemHeight = fixedItemHeight;
		}
		
		public FungusVariableListAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f) 
		{}

		public int Count 
		{
			get { return _arrayProperty.arraySize; }
		}
		
		public virtual bool CanDrag(int index) 
		{
			return true;
		}

		public virtual bool CanRemove(int index) 
		{
			return true;
		}
		
		public void Add() 
		{
			int newIndex = _arrayProperty.arraySize;
			++_arrayProperty.arraySize;
			ResetValue(_arrayProperty.GetArrayElementAtIndex(newIndex));
		}

		public void Insert(int index) 
		{
			_arrayProperty.InsertArrayElementAtIndex(index);
			ResetValue(_arrayProperty.GetArrayElementAtIndex(index));
		}

		public void Duplicate(int index) 
		{
			_arrayProperty.InsertArrayElementAtIndex(index);
		}

		public void Remove(int index) 
		{
			FungusVariable variable = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as FungusVariable;
			Undo.DestroyObjectImmediate(variable);

			_arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue = null;
			_arrayProperty.DeleteArrayElementAtIndex(index);
		}

		public void Move(int sourceIndex, int destIndex) 
		{
			if (destIndex > sourceIndex)
				--destIndex;
			_arrayProperty.MoveArrayElement(sourceIndex, destIndex);
		}

		public void Clear() {
			_arrayProperty.ClearArray();
		}
		
		public virtual void DrawItem(Rect position, int index) 
		{
			FungusVariable variable = this[index].objectReferenceValue as FungusVariable;

			float width1 = 60;
			float width3 = 50;
			float width2 = Mathf.Max(position.width - width1 - width3, 100);
			
			Rect typeRect = position;
			typeRect.width = width1;
			
			Rect keyRect = position;
			keyRect.x += width1;
			keyRect.width = width2;
			
			Rect scopeRect = position;
			scopeRect.x += width1 + width2;
			scopeRect.width = width3;

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

			GUI.Label(typeRect, type);

			EditorGUI.BeginChangeCheck();

			string key = variable.key;

			if (Application.isPlaying)
			{
				const float w = 100;
				Rect valueRect = keyRect;
				keyRect.width = w - 5;
				valueRect.x += w;
				valueRect.width -= (w + 5);
				key = EditorGUI.TextField(keyRect, variable.key);
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
			}
			else
			{
				keyRect.width -= 5;
				key = EditorGUI.TextField(keyRect, variable.key);
			}

			VariableScope scope = (VariableScope)EditorGUI.EnumPopup(scopeRect, variable.scope);
		
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject(variable, "Set Variable");

				char[] arr = key.Where(c => (char.IsLetterOrDigit(c) || c == '_')).ToArray(); 
				key = new string(arr);

				variable.key = key;
				variable.scope = scope;
			}
		}
		
		public virtual float GetItemHeight(int index) 
		{
			return fixedItemHeight != 0f
				? fixedItemHeight
					: EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false)
					;
		}
		
		private void ResetValue(SerializedProperty element) 
		{
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
