using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(AddOption))]
	public class AddOptionEditor : SetVariableEditor
	{
		protected SerializedProperty optionTextProp;
		protected SerializedProperty hideOnSelectedProp;
		protected SerializedProperty targetBlockProp;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			optionTextProp = serializedObject.FindProperty("optionText");
			hideOnSelectedProp = serializedObject.FindProperty("hideOnSelected");
			targetBlockProp = serializedObject.FindProperty("targetBlock");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();
			
			AddOption t = target as AddOption;
			
			EditorGUILayout.PropertyField(optionTextProp, new GUIContent("Option Text", "Text to display on the option button."));
			
			BlockEditor.BlockField(targetBlockProp,
			                             new GUIContent("Target Block", "Block to execute when this option is selected by the player."),
			                             new GUIContent("<Continue>"),
			                             t.GetFlowchart());

			serializedObject.ApplyModifiedProperties();

			base.DrawCommandGUI();
			
			serializedObject.Update();
			EditorGUILayout.PropertyField(hideOnSelectedProp, new GUIContent("Hide On Selected", "Hide this option forever once the player has selected it."));
			serializedObject.ApplyModifiedProperties();
		}
	}
	
}