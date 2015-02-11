using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(Option))]
	public class OptionEditor : IfEditor
	{
		protected SerializedProperty optionTextProp;
		protected SerializedProperty hideOnSelectedProp;
		protected SerializedProperty hideOnConditionProp;

		protected override void OnEnable()
		{
			base.OnEnable();
			optionTextProp = serializedObject.FindProperty("optionText");
			hideOnSelectedProp = serializedObject.FindProperty("hideOnSelected");
			hideOnConditionProp = serializedObject.FindProperty("hideOnCondition");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(optionTextProp);
			EditorGUILayout.PropertyField(hideOnSelectedProp);
			EditorGUILayout.PropertyField(hideOnConditionProp);

			serializedObject.ApplyModifiedProperties();

			if (hideOnConditionProp.boolValue)
			{
				EditorGUI.indentLevel++;
				base.DrawCommandGUI();
				EditorGUI.indentLevel--;
			}
		}
	}
	
}