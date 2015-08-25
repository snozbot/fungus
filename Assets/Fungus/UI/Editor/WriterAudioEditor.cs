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
		protected SerializedProperty audioSourceProp;
		protected SerializedProperty audioModeProp;
		protected SerializedProperty beepSoundsProp;
		protected SerializedProperty soundEffectProp;
		protected SerializedProperty beepDelayProp;
		protected SerializedProperty randomizeDelayProp;

		protected virtual void OnEnable()
		{
			volumeProp = serializedObject.FindProperty("volume");
			loopProp = serializedObject.FindProperty("loop");
			audioSourceProp = serializedObject.FindProperty("audioSource");
			audioModeProp = serializedObject.FindProperty("audioMode");
			beepSoundsProp = serializedObject.FindProperty("beepSounds");
			soundEffectProp = serializedObject.FindProperty("soundEffect");
			beepDelayProp = serializedObject.FindProperty("beepDelay");
			randomizeDelayProp = serializedObject.FindProperty("randomizeDelay");
		}

		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(volumeProp);
			EditorGUILayout.PropertyField(loopProp);
			EditorGUILayout.PropertyField(audioSourceProp);

			EditorGUILayout.PropertyField(audioModeProp);
			if ((WriterAudio.AudioMode)audioModeProp.enumValueIndex == WriterAudio.AudioMode.Beeps)
			{
				ReorderableListGUI.Title(new GUIContent("Beep Sounds", "A list of beep sounds to play at random"));
				ReorderableListGUI.ListField(beepSoundsProp);

				EditorGUILayout.PropertyField(beepDelayProp);
				EditorGUILayout.PropertyField(randomizeDelayProp);
			}
			else
			{
				EditorGUILayout.PropertyField(soundEffectProp);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}