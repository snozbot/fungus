using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.Script
{

	[CustomPropertyDrawer (typeof(Variable))]
	public class VariableDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty keyProp = property.FindPropertyRelative("key");
			SerializedProperty typeProp = property.FindPropertyRelative("type");
			SerializedProperty scopeProp = property.FindPropertyRelative("scope");
			
			// Draw the text field control GUI.
			EditorGUI.BeginChangeCheck();
			
			float width2 = 60;
			float width3 = 50;
			float width1 = position.width - width2 - width3;

			Rect keyRect = position;
			keyRect.width = width1;
			
			Rect typeRect = position;
			typeRect.x += width1;
			typeRect.width = width2;
			
			Rect scopeRect = position;
			scopeRect.x += width1 + width2;
			scopeRect.width = width3;
			
			string keyValue = EditorGUI.TextField(keyRect, label, keyProp.stringValue);
			int typeValue = (int)(VariableType)EditorGUI.EnumPopup(typeRect, (VariableType)typeProp.enumValueIndex);
			int scopeValue = (int)(VariableScope)EditorGUI.EnumPopup(scopeRect, (VariableScope)scopeProp.enumValueIndex);
			
			if (EditorGUI.EndChangeCheck ())
			{
				char[] arr = keyValue.Where(c => (char.IsLetterOrDigit(c) || c == '_')).ToArray(); 
				
				keyValue = new string(arr);
				
				keyProp.stringValue = keyValue;
				typeProp.enumValueIndex = typeValue;	
				scopeProp.enumValueIndex = scopeValue;
			}
			
			EditorGUI.EndProperty();
		}
	}
	
	[CustomEditor (typeof(FungusVariable), true)]
	public class FungusVariableEditor : FungusCommandEditor
	{
		public override void OnInspectorGUI() 
		{
			FungusVariable t = target as FungusVariable;

			EditorGUI.BeginChangeCheck();

			string key = EditorGUILayout.TextField(new GUIContent("Key", "Name to use for this variable"), t.key);
			VariableScope scope = (VariableScope)EditorGUILayout.EnumPopup(new GUIContent("Scope", "Local or global access to variable value"), t.scope);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Variable");
				t.key = key;
				t.scope = scope;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Delete Variable"))
			{
				Undo.RecordObject(t, "Delete Variable");
				DestroyImmediate(t);
			}
			GUILayout.EndHorizontal();
		}

		static public FungusVariable VariableField(GUIContent label, FungusScript fungusScript, FungusVariable variable, Func<FungusVariable, bool> filter = null)
		{
			List<string> variableKeys = new List<string>();
			List<FungusVariable> variableObjects = new List<FungusVariable>();
			
			variableKeys.Add("<None>");
			variableObjects.Add(null);
			
			FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
			int index = 0;
			int selectedIndex = 0;
			foreach (FungusVariable v in variables)
			{
				if (filter != null)
				{
					if (!filter(v))
					{
						continue;
					}
				}
				
				variableKeys.Add(v.key);
				variableObjects.Add(v);
				
				index++;
				
				if (v == variable)
				{
					selectedIndex = index;
				}
			}
			
			selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());
			
			return variableObjects[selectedIndex];
		}
	}

	[CustomPropertyDrawer (typeof(BooleanData))]
	public class BooleanDataDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty referenceProp = property.FindPropertyRelative("booleanReference");
			SerializedProperty valueProp = property.FindPropertyRelative("booleanValue");

			const int popupWidth = 65;
			
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;

			if (referenceProp.objectReferenceValue == null)
			{
				valueProp.boolValue = EditorGUI.Toggle(valueRect, valueProp.boolValue);
				popupRect.x += valueRect.width + 5;
				popupRect.width = popupWidth;
			}

			FungusScript fungusScript = property.serializedObject.targetObject as FungusScript;
			if (fungusScript == null)
			{
				FungusCommand command = property.serializedObject.targetObject as FungusCommand;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}

			if (fungusScript != null)
			{
				BooleanVariable selectedBooleanVariable = referenceProp.objectReferenceValue as BooleanVariable;

				List<string> variableKeys = new List<string>();
				List<FungusVariable> variableObjects = new List<FungusVariable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
				int index = 0;
				int selectedIndex = 0;
				foreach (FungusVariable v in variables)
				{
					if (v.GetType() != typeof(BooleanVariable))
					{
						continue;
					}

					variableKeys.Add(v.key);
					variableObjects.Add(v);
					
					index++;
					
					if (v == selectedBooleanVariable)
					{
						selectedIndex = index;
					}
				}
				
				selectedIndex = EditorGUI.Popup(popupRect, selectedIndex, variableKeys.ToArray());
				referenceProp.objectReferenceValue = variableObjects[selectedIndex];
			}

			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer (typeof(IntegerData))]
	public class IntegerDataDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty referenceProp = property.FindPropertyRelative("integerReference");
			SerializedProperty valueProp = property.FindPropertyRelative("integerValue");

			const int popupWidth = 65;
			
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;

			if (referenceProp.objectReferenceValue == null)
			{
				valueProp.intValue = EditorGUI.IntField(valueRect, valueProp.intValue);
				popupRect.x += valueRect.width + 5;
				popupRect.width = popupWidth;
			}

			FungusScript fungusScript = property.serializedObject.targetObject as FungusScript;
			if (fungusScript == null)
			{
				FungusCommand command = property.serializedObject.targetObject as FungusCommand;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}

			if (fungusScript != null)
			{
				IntegerVariable selectedVariable = referenceProp.objectReferenceValue as IntegerVariable;
				
				List<string> variableKeys = new List<string>();
				List<FungusVariable> variableObjects = new List<FungusVariable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
				int index = 0;
				int selectedIndex = 0;
				foreach (FungusVariable v in variables)
				{
					if (v.GetType() != typeof(IntegerVariable))
					{
						continue;
					}
					
					variableKeys.Add(v.key);
					variableObjects.Add(v);
					
					index++;
					
					if (v == selectedVariable)
					{
						selectedIndex = index;
					}
				}
				
				selectedIndex = EditorGUI.Popup(popupRect, selectedIndex, variableKeys.ToArray());
				referenceProp.objectReferenceValue = variableObjects[selectedIndex];
			}

			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer (typeof(FloatData))]
	public class FloatDataDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty referenceProp = property.FindPropertyRelative("floatReference");
			SerializedProperty valueProp = property.FindPropertyRelative("floatValue");

			const int popupWidth = 65;

			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;
			
			if (referenceProp.objectReferenceValue == null)
			{
				valueProp.floatValue = EditorGUI.FloatField(valueRect, valueProp.floatValue);
				popupRect.x += valueRect.width + 5;
				popupRect.width = popupWidth;
			}
			
			FungusScript fungusScript = property.serializedObject.targetObject as FungusScript;
			if (fungusScript == null)
			{
				FungusCommand command = property.serializedObject.targetObject as FungusCommand;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}
			
			if (fungusScript != null)
			{
				FloatVariable selectedVariable = referenceProp.objectReferenceValue as FloatVariable;
				
				List<string> variableKeys = new List<string>();
				List<FungusVariable> variableObjects = new List<FungusVariable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
				int index = 0;
				int selectedIndex = 0;
				foreach (FungusVariable v in variables)
				{
					if (v.GetType() != typeof(FloatVariable))
					{
						continue;
					}
					
					variableKeys.Add(v.key);
					variableObjects.Add(v);
					
					index++;
					
					if (v == selectedVariable)
					{
						selectedIndex = index;
					}
				}
				
				selectedIndex = EditorGUI.Popup(popupRect, selectedIndex, variableKeys.ToArray());
				referenceProp.objectReferenceValue = variableObjects[selectedIndex];
			}
			
			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer (typeof(StringData))]
	public class StringDataDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty referenceProp = property.FindPropertyRelative("stringReference");
			SerializedProperty valueProp = property.FindPropertyRelative("stringValue");
			
			const int popupWidth = 65;
			
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;
			
			if (referenceProp.objectReferenceValue == null)
			{
				valueProp.stringValue = EditorGUI.TextField(valueRect, valueProp.stringValue);
				popupRect.x += valueRect.width + 5;
				popupRect.width = popupWidth;
			}
			
			FungusScript fungusScript = property.serializedObject.targetObject as FungusScript;
			if (fungusScript == null)
			{
				FungusCommand command = property.serializedObject.targetObject as FungusCommand;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}
			
			if (fungusScript != null)
			{
				StringVariable selectedVariable = referenceProp.objectReferenceValue as StringVariable;
				
				List<string> variableKeys = new List<string>();
				List<FungusVariable> variableObjects = new List<FungusVariable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
				int index = 0;
				int selectedIndex = 0;
				foreach (FungusVariable v in variables)
				{
					if (v.GetType() != typeof(StringVariable))
					{
						continue;
					}
					
					variableKeys.Add(v.key);
					variableObjects.Add(v);
					
					index++;
					
					if (v == selectedVariable)
					{
						selectedIndex = index;
					}
				}
				
				selectedIndex = EditorGUI.Popup(popupRect, selectedIndex, variableKeys.ToArray());
				referenceProp.objectReferenceValue = variableObjects[selectedIndex];
			}
			
			EditorGUI.EndProperty();
		}
	}
	
}