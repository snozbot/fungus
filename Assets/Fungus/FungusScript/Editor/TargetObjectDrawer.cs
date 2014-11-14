using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	[CustomPropertyDrawer (typeof(TargetObject), true)]
	public class TargetObjectDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty targetTypeProp = property.FindPropertyRelative("targetType");
			SerializedProperty otherGameObjectProp = property.FindPropertyRelative("otherGameObject");

			EditorGUI.PropertyField(position, targetTypeProp, new GUIContent("Game Object", "Select either the owner game object or another object in the scene."));

			if (targetTypeProp.enumValueIndex == 0)
			{
				otherGameObjectProp.objectReferenceValue = null;
			}
			else
			{
				Rect objectFieldRect = position;
				objectFieldRect.y += EditorGUIUtility.singleLineHeight;
				objectFieldRect.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(objectFieldRect, otherGameObjectProp, new GUIContent(" "));
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Adjust property height if also showing an object reference field
			float propHeight = base.GetPropertyHeight(property, label);
			SerializedProperty targetTypeProp = property.FindPropertyRelative("targetType");
			if (targetTypeProp.enumValueIndex == 1)
			{
				return propHeight * 2;
			}
			return propHeight;
		}
	}

}