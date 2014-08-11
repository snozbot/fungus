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
	
	public class FungusVariableListAdaptor : SerializedPropertyAdaptor 
	{
		public FungusVariableListAdaptor(SerializedProperty arrayProperty, float fixedItemHeight) : base(arrayProperty, fixedItemHeight)
		{}
		
		public FungusVariableListAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f) 
		{}

		public override void DrawItem(Rect position, int index) 
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
		
	}
	
}
