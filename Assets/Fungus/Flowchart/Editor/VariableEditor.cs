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

		static public void VariableField(SerializedProperty property, 
		                                 GUIContent label, 
		                                 Flowchart flowchart, 
		                                 Func<Variable, bool> filter, 
		                                 Func<string, int, string[], int> drawer = null)
		{
			List<string> variableKeys = new List<string>();
			List<Variable> variableObjects = new List<Variable>();
			
			variableKeys.Add("<None>");
			variableObjects.Add(null);
			
			List<Variable> variables = flowchart.variables;
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

			Flowchart[] fsList = GameObject.FindObjectsOfType<Flowchart>();
			foreach (Flowchart fs in fsList)
			{
				if (fs == flowchart)
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

			if (drawer == null)
			{
				selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());
			}
			else
			{
				selectedIndex = drawer(label.text, selectedIndex, variableKeys.ToArray());
			}

			property.objectReferenceValue = variableObjects[selectedIndex];
		}
	}

	[CustomPropertyDrawer(typeof(VariablePropertyAttribute))]
	public class VariableDrawer : PropertyDrawer
	{	
		
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			VariablePropertyAttribute variableProperty = attribute as VariablePropertyAttribute;
			if (variableProperty == null)
			{
				return;
			}

			EditorGUI.BeginProperty(position, label, property);

			// Filter the variables by the types listed in the VariableProperty attribute
			Func<Variable, bool> compare = v => 
			{
				if (v == null)
				{
					return false;
				}

				if (variableProperty.VariableTypes.Length == 0)
				{
					return true;
				}

				return variableProperty.VariableTypes.Contains<System.Type>(v.GetType());
			};

			VariableEditor.VariableField(property, 
			                             label,
			                             FlowchartWindow.GetFlowchart(),
			                             compare,
			                             (s,t,u) => (EditorGUI.Popup(position, s, t, u)));

			EditorGUI.EndProperty();
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
			
			Flowchart flowchart = property.serializedObject.targetObject as Flowchart;
			if (flowchart == null)
			{
				Command command = property.serializedObject.targetObject as Command;
				if (command != null)
				{
					flowchart = command.GetFlowchart();
				}
			}
			
			if (flowchart != null)
			{
				T selectedVariable = referenceProp.objectReferenceValue as T;
				
				List<string> variableKeys = new List<string>();
				List<Variable> variableObjects = new List<Variable>();
				
				variableKeys.Add("<Value>");
				variableObjects.Add(null);
				
				int index = 0;
				int selectedIndex = 0;
				foreach (Variable v in flowchart.variables)
				{
					if (v == null)
					{
						continue;
					}

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