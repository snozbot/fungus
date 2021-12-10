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
        protected SerializedProperty useLegacyAudioLogicProp;

        protected virtual void OnEnable()
        {
            volumeProp = serializedObject.FindProperty("volume");
            loopProp = serializedObject.FindProperty("loop");
            targetAudioSourceProp = serializedObject.FindProperty("targetAudioSource");
            inputSoundProp = serializedObject.FindProperty("inputSound");
            audioModeProp = serializedObject.FindProperty("audioMode");
            beepSoundsProp = serializedObject.FindProperty("beepSounds");
            soundEffectProp = serializedObject.FindProperty("soundEffect");
            useLegacyAudioLogicProp = serializedObject.FindProperty("useLegacyAudioLogic");
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(volumeProp);
            EditorGUILayout.PropertyField(loopProp);
            EditorGUILayout.PropertyField(useLegacyAudioLogicProp);
            if (useLegacyAudioLogicProp.boolValue)
            {
                EditorGUILayout.PropertyField(targetAudioSourceProp);
            }
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

            if( ( ((MonoBehaviour)target).GetComponent<AudioSource>() != null || targetAudioSourceProp.objectReferenceValue != null) &&
                !useLegacyAudioLogicProp.boolValue)
            {
                EditorGUILayout.HelpBox("AudioSource found or targetAudioSource set but will not be used. " +
                    "\nToggle useLegacyAudioLogic to use the targetAudioSource and not the AudioSources that might be provided by the Character", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }    
}