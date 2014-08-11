using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.Script
{
	[CustomEditor (typeof(FungusVariable), true)]
	public class FungusVariableEditor : FungusCommandEditor
	{
		void OnEnable()
		{
			FungusVariable t = target as FungusVariable;
			t.hideFlags = HideFlags.HideInInspector;
		}

		static public FungusVariable VariableField(GUIContent label, FungusScript fungusScript, FungusVariable variable, Func<FungusVariable, bool> filter = null)
		{
			List<string> variableKeys = new List<string>();
			List<FungusVariable> variableObjects = new List<FungusVariable>();
			
			variableKeys.Add("<None>");
			variableObjects.Add(null);
			
			List<FungusVariable> variables = fungusScript.variables;
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