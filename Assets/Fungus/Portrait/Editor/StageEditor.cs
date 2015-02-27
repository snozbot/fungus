using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;


namespace Fungus
{
	
	[CustomEditor (typeof(ControlStage))]
	public class StageEditor : CommandEditor
	{
		protected SerializedProperty displayProp;
		protected SerializedProperty portraitStageProp;
		protected SerializedProperty replacedPortraitStageProp;
		protected SerializedProperty useDefaultSettingsProp;
		protected SerializedProperty fadeDurationProp;
		protected SerializedProperty waitUntilFinishedProp;
		
		protected virtual void OnEnable()
		{
			displayProp = serializedObject.FindProperty("display");
			portraitStageProp = serializedObject.FindProperty("portraitStage");
			replacedPortraitStageProp = serializedObject.FindProperty("replacedPortraitStage");
			useDefaultSettingsProp = serializedObject.FindProperty("useDefaultSettings");
			fadeDurationProp = serializedObject.FindProperty("fadeDuration");
			waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();
			
			ControlStage t = target as ControlStage;

			// Format Enum names
			string[] displayLabels = StringFormatter.FormatEnumNames(t.display,"<None>");
			displayProp.enumValueIndex = EditorGUILayout.Popup("Display", (int)displayProp.enumValueIndex, displayLabels);
			
			string replaceLabel = "Portrait Stage";
			if (t.display == StageDisplayType.Swap)
			{
				CommandEditor.ObjectField<PortraitStage>(replacedPortraitStageProp, 
				                                     new GUIContent("Replace", "Character to swap with"), 
				                                     new GUIContent("<Default>"),
				                                     PortraitStage.activePortraitStages);
				replaceLabel = "With";
			}

			if (PortraitStage.activePortraitStages.Count > 1)
			{
				CommandEditor.ObjectField<PortraitStage>(portraitStageProp, 
				                                         new GUIContent(replaceLabel, "Stage to display the character portraits on"), 
				                                         new GUIContent("<Default>"),
				                                         PortraitStage.activePortraitStages);
			}

			bool showOptionalFields = true;
			PortraitStage ps = t.portraitStage;
			// Only show optional portrait fields once required fields have been filled...
			if (t.portraitStage != null)                // Character is selected
			{
				if (t.portraitStage == null)        // If no default specified, try to get any portrait stage in the scene
				{
					ps = GameObject.FindObjectOfType<PortraitStage>();
				}
				if (ps == null)
				{
					EditorGUILayout.HelpBox("No portrait stage has been set. Please create a new portrait stage using [Game Object > Fungus > Portrait > Portrait Stage].", MessageType.Error);
					showOptionalFields = false; 
				}
			}
			if (t.display != StageDisplayType.None && showOptionalFields) 
			{
				EditorGUILayout.PropertyField(useDefaultSettingsProp);
				if (!t.useDefaultSettings)
				{
					EditorGUILayout.PropertyField(fadeDurationProp);
				}
				EditorGUILayout.PropertyField(waitUntilFinishedProp);
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}