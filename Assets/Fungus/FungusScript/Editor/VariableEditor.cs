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

		public static VariableInfoAttribute GetVariableInfo(System.Type variableType)
		{
			object[] attributes = variableType.GetCustomAttributes(typeof(VariableInfoAttribute), false);
			foreach (object obj in attributes)
			{
				VariableInfoAttribute variableInfoAttr = obj as VariableInfoAttribute;
				if (variableInfoAttr != null)
				{
					return variableInfoAttr;
				}
			}
			
			return null;
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

			FungusScript[] fsList = GameObject.FindObjectsOfType<FungusScript>();
			foreach (FungusScript fs in fsList)
			{
				if (fs == fungusScript)
				{
					continue;
				}

				List<Variable> publicVars = fs.GetPublicVariables();
				foreach (Variable v in publicVars)
				{
					variableKeys.Add(fs.name + " / " + v.key);
					variableObjects.Add(v);

					index++;

					if (v == selectedVariable)
					{
						selectedIndex = index;
					}
				}
			}

			selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());
			
			property.objectReferenceValue = variableObjects[selectedIndex];
		}
	}

	public class VariableDataDrawer<T> : PropertyDrawer where T : Variable
	{	

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);

			// The variable reference and data properties must follow the naming convention 'typeRef', 'typeVal'

			VariableInfoAttribute typeInfo = VariableEditor.GetVariableInfo(typeof(T));
			if (typeInfo == null)
			{
				return;
			}

			string propNameBase = typeInfo.VariableType;
			propNameBase = Char.ToLowerInvariant(propNameBase[0]) + propNameBase.Substring(1);

			SerializedProperty referenceProp = property.FindPropertyRelative(propNameBase + "Ref");
			SerializedProperty valueProp = property.FindPropertyRelative(propNameBase + "Val");

			if (referenceProp == null || valueProp == null)
			{
				return;
			}

			const int popupWidth = 65;
			
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			Rect valueRect = controlRect;
			valueRect.width = controlRect.width - popupWidth - 5;
			Rect popupRect = controlRect;
			
			if (referenceProp.objectReferenceValue == null)
			{
				EditorGUI.PropertyField(valueRect, valueProp, new GUIContent(""));
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
				T selectedVariable = referenceProp.objectReferenceValue as T;
				
				List<string> variableKeys = new List<string>();
				List<Variable> variableObjects = new List<Variable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				int index = 0;
				int selectedIndex = 0;
				foreach (Variable v in fungusScript.variables)
				{
					if (v.GetType() != typeof(T))
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

	[CustomPropertyDrawer (typeof(BooleanData))]
	public class BooleanDataDrawer : VariableDataDrawer<BooleanVariable>
	{}

	[CustomPropertyDrawer (typeof(IntegerData))]
	public class IntegerDataDrawer : VariableDataDrawer<IntegerVariable>
	{}

	[CustomPropertyDrawer (typeof(FloatData))]
	public class FloatDataDrawer : VariableDataDrawer<FloatVariable>
	{}

	[CustomPropertyDrawer (typeof(StringData))]
	public class StringDataDrawer : VariableDataDrawer<StringVariable>
	{}

	[CustomPropertyDrawer (typeof(ColorData))]
	public class ColorDataDrawer : VariableDataDrawer<ColorVariable>
	{}

	[CustomPropertyDrawer (typeof(Vector2Data))]
	public class Vector2DataDrawer : VariableDataDrawer<Vector2Variable>
	{}

	[CustomPropertyDrawer (typeof(Vector3Data))]
	public class Vector3DataDrawer : VariableDataDrawer<Vector3Variable>
	{}
	
	[CustomPropertyDrawer (typeof(MaterialData))]
	public class MaterialDataDrawer : VariableDataDrawer<MaterialVariable>
	{}

	[CustomPropertyDrawer (typeof(TextureData))]
	public class TextureDataDrawer : VariableDataDrawer<TextureVariable>
	{}

	[CustomPropertyDrawer (typeof(SpriteData))]
	public class SpriteDataDrawer : VariableDataDrawer<SpriteVariable>
	{}

	[CustomPropertyDrawer (typeof(GameObjectData))]
	public class GameObjectDataDrawer : VariableDataDrawer<GameObjectVariable>
	{}
	
	[CustomPropertyDrawer (typeof(ObjectData))]
	public class ObjectDataDrawer : VariableDataDrawer<ObjectVariable>
	{}
}