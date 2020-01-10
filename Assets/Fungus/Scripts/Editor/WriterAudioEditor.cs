// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
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
        protected SerializedProperty inputSoundProp;

        protected virtual void OnEnable()
        {
            volumeProp = serializedObject.FindProperty("volume");
            loopProp = serializedObject.FindProperty("loop");
            targetAudioSourceProp = serializedObject.FindProperty("targetAudioSource");
            inputSoundProp = serializedObject.FindProperty("inputSound");
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
            EditorGUILayout.PropertyField(inputSoundProp);

            EditorGUILayout.PropertyField(audioModeProp);
            if ((AudioMode)audioModeProp.enumValueIndex == AudioMode.Beeps)
            {
                EditorGUILayout.PropertyField(beepSoundsProp, new GUIContent("Beep Sounds", "A list of beep sounds to play at random"),true);
            }
            else
            {
                EditorGUILayout.PropertyField(soundEffectProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }    
}