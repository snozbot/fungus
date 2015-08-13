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
	
	[CustomEditor (typeof(ControlAudio))]
	public class ControlAudioEditor : CommandEditor
	{
		protected SerializedProperty controlProp;
		protected SerializedProperty audioSourceProp;
		protected SerializedProperty startVolumeProp; 
		protected SerializedProperty endVolumeProp; 
		protected SerializedProperty fadeDurationProp; 
		protected SerializedProperty waitUntilFinishedProp;
		
		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			controlProp = serializedObject.FindProperty("control");
			audioSourceProp = serializedObject.FindProperty("audioSource");
			startVolumeProp = serializedObject.FindProperty("startVolume");
			endVolumeProp = serializedObject.FindProperty("endVolume");
			fadeDurationProp = serializedObject.FindProperty("fadeDuration");
			waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();

			ControlAudio t = target as ControlAudio;

			EditorGUILayout.PropertyField(controlProp);
			EditorGUILayout.PropertyField(audioSourceProp);
			string fadeLabel = "Fade Out Duration";
			if (t.control != ControlAudio.controlType.StopLoop && t.control != ControlAudio.controlType.PauseLoop)
			{
				fadeLabel = "Fade In Duration";
				string volumeLabel = "End Volume";
				if (t.control == ControlAudio.controlType.ChangeVolume)
				{
					fadeLabel = "Fade Duration";
					volumeLabel = "New Volume";
				}
				EditorGUILayout.PropertyField(endVolumeProp,new GUIContent(volumeLabel));
			}
			EditorGUILayout.PropertyField(fadeDurationProp,new GUIContent(fadeLabel));
			EditorGUILayout.PropertyField(waitUntilFinishedProp);
			serializedObject.ApplyModifiedProperties();
		}
	}
}