using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(Fungus.Menu))]
	public class MenuEditor : CommandEditor 
	{
		protected SerializedProperty textProp;
		protected SerializedProperty descriptionProp;
		protected SerializedProperty targetBlockProp;
		protected SerializedProperty hideIfVisitedProp;
		protected SerializedProperty interactableProp;
		protected SerializedProperty setMenuDialogProp;

		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			textProp = serializedObject.FindProperty("text");
			descriptionProp = serializedObject.FindProperty("description");
			targetBlockProp = serializedObject.FindProperty("targetBlock");
			hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
			interactableProp = serializedObject.FindProperty("interactable");
			setMenuDialogProp = serializedObject.FindProperty("setMenuDialog");
		}
		
		public override void DrawCommandGUI()
		{
			Flowchart flowchart = FlowchartWindow.GetFlowchart();
			if (flowchart == null)
			{
				return;
			}
			
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(textProp);

			EditorGUILayout.PropertyField(descriptionProp);
			
			BlockEditor.BlockField(targetBlockProp,
			                             new GUIContent("Target Block", "Block to call when option is selected"), 
			                             new GUIContent("<None>"), 
			                             flowchart);
			
			EditorGUILayout.PropertyField(hideIfVisitedProp);
			EditorGUILayout.PropertyField(interactableProp);
			EditorGUILayout.PropertyField(setMenuDialogProp);

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}
