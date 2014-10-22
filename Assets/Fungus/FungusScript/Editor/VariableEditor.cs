using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fungus
{
	[CustomEditor (typeof(Variable), true)]
	public class VariableEditor : CommandEditor
	{
		protected virtual void OnEnable()
		{
			Variable t = target as Variable;
			t.hideFlags = HideFlags.HideInInspector;
		}

		static public void VariableField(SerializedProperty property, GUIContent label, FungusScript fungusScript, Func<Variable, bool> filter = null)
		{
			List<string> variableKeys = new List<string>();
			List<Variable> variableObjects = new List<Variable>();
			
			variableKeys.Add("<None>");
			variableObjects.Add(null);
			
			List<Variable> variables = fungusScript.variables;
			int index = 0;
			int selectedIndex = 0;

			Variable selectedVariable = property.objectReferenceValue as Variable;

			foreach (Variable v in variables)
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
				
				if (v == selectedVariable)
				{
					selectedIndex = index;
				}
			}
			
			selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());
			
			property.objectReferenceValue = variableObjects[selectedIndex];
		}
	}

	[CustomPropertyDrawer (typeof(BooleanData))]
	public class BooleanDataDrawer : PropertyDrawer 
	{	
		protected enum BooleanState
		{
			True,
			False
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty referenceProp = property.FindPropertyRelative("booleanRef");
			SerializedProperty valueProp = property.FindPropertyRelative("booleanVal");

			const int popupWidth = 65;
			
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;

			if (referenceProp.objectReferenceValue == null)
			{
				valueProp.boolValue = ((BooleanState)EditorGUI.EnumPopup(valueRect, valueProp.boolValue ? BooleanState.True : BooleanState.False) == BooleanState.True);
				popupRect.x += valueRect.width + 5;
				popupRect.width = popupWidth;
			}

			FungusScript fungusScript = property.serializedObject.targetObject as FungusScript;
			if (fungusScript == null)
			{
				Command command = property.serializedObject.targetObject as Command;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}

			if (fungusScript != null)
			{
				BooleanVariable selectedBooleanVariable = referenceProp.objectReferenceValue as BooleanVariable;

				List<string> variableKeys = new List<string>();
				List<Variable> variableObjects = new List<Variable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				int index = 0;
				int selectedIndex = 0;
				foreach (Variable v in fungusScript.variables)
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
			
			SerializedProperty referenceProp = property.FindPropertyRelative("integerRef");
			SerializedProperty valueProp = property.FindPropertyRelative("integerVal");

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
				Command command = property.serializedObject.targetObject as Command;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}

			if (fungusScript != null)
			{
				IntegerVariable selectedVariable = referenceProp.objectReferenceValue as IntegerVariable;
				
				List<string> variableKeys = new List<string>();
				List<Variable> variableObjects = new List<Variable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				int index = 0;
				int selectedIndex = 0;
				foreach (Variable v in fungusScript.variables)
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
			
			SerializedProperty referenceProp = property.FindPropertyRelative("floatRef");
			SerializedProperty valueProp = property.FindPropertyRelative("floatVal");

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
				Command command = property.serializedObject.targetObject as Command;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}
			
			if (fungusScript != null)
			{
				FloatVariable selectedVariable = referenceProp.objectReferenceValue as FloatVariable;
				
				List<string> variableKeys = new List<string>();
				List<Variable> variableObjects = new List<Variable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				int index = 0;
				int selectedIndex = 0;
				foreach (Variable v in fungusScript.variables)
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
			
			SerializedProperty referenceProp = property.FindPropertyRelative("stringRef");
			SerializedProperty valueProp = property.FindPropertyRelative("stringVal");
			
			const int popupWidth = 65;
			
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;
			
			if (referenceProp.objectReferenceValue == null)
			{
				// StringData stringData = valueProp.serializedObject
				valueProp.stringValue = EditorGUI.TextField(valueRect, valueProp.stringValue);
				popupRect.x += valueRect.width + 5;
				popupRect.width = popupWidth;
			}
			
			FungusScript fungusScript = property.serializedObject.targetObject as FungusScript;
			if (fungusScript == null)
			{
				Command command = property.serializedObject.targetObject as Command;
				if (command != null)
				{
					fungusScript = command.GetFungusScript();
				}
			}
			
			if (fungusScript != null)
			{
				StringVariable selectedVariable = referenceProp.objectReferenceValue as StringVariable;
				
				List<string> variableKeys = new List<string>();
				List<Variable> variableObjects = new List<Variable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				int index = 0;
				int selectedIndex = 0;
				foreach (Variable v in fungusScript.variables)
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