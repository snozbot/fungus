using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(WriterAudio))]
	public class WriterAudioEditor : Editor
	{
		protected SerializedProperty volumeProp;
		protected SerializedProperty loopProp;
		protected SerializedProperty targetAudioSourceProp;
		protected SerializedProperty audioModeProp;
		protected SerializedProperty beepSoundsProp;
		protected SerializedProperty soundEffectProp;

		protected virtual void OnEnable()
		{
			volumeProp = serializedObject.FindProperty("volume");
			loopProp = serializedObject.FindProperty("loop");
			targetAudioSourceProp = serializedObject.FindProperty("targetAudioSource");
			audioModeProp = serializedObject.FindProperty("audioMode");
			beepSoundsProp = serializedObject.FindProperty("beepSounds");
			soundEffectProp = serializedObject.FindProperty("soundEffect");
		}

		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(volumeProp);
			EditorGUILayout.PropertyField(loopProp);
			EditorGUILayout.PropertyField(targetAudioSourceProp);

			EditorGUILayout.PropertyField(audioModeProp);
			if ((WriterAudio.AudioMode)audioModeProp.enumValueIndex == WriterAudio.AudioMode.Beeps)
			{
				ReorderableListGUI.Title(new GUIContent("Beep Sounds", "A list of beep sounds to play at random"));
				ReorderableListGUI.ListField(beepSoundsProp);
			}
			else
			{
				EditorGUILayout.PropertyField(soundEffectProp);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}