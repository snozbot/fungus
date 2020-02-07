// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
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
        
        public override void OnEnable()
        {
            base.OnEnable();

            controlProp = serializedObject.FindProperty("control");
            audioSourceProp = serializedObject.FindProperty("_audioSource");
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
            if (t.Control != ControlAudioType.StopLoop && t.Control != ControlAudioType.PauseLoop)
            {
                fadeLabel = "Fade In Duration";
                string volumeLabel = "End Volume";
                if (t.Control == ControlAudioType.ChangeVolume)
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