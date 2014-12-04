using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Fungus
{

	[CustomEditor (typeof(RandomInteger))]
	public class RandomIntegerEditor : CommandEditor 
	{
		protected SerializedProperty variableProp;
		protected SerializedProperty minValueProp;
		protected SerializedProperty maxValueProp;

		protected virtual void OnEnable()
		{
			variableProp = serializedObject.FindProperty("variable");
			minValueProp = serializedObject.FindProperty("minValue");
			maxValueProp = serializedObject.FindProperty("maxValue");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			RandomInteger t = target as RandomInteger;

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			VariableEditor.VariableField(variableProp, 
			                             new GUIContent("Variable", "Variable to use in operation"),
										 t.GetFungusScript(),
										 (v) => (v.GetType() == typeof(IntegerVariable)));

			EditorGUILayout.PropertyField(minValueProp);
			EditorGUILayout.PropertyField(maxValueProp);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
