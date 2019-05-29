// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-05-29 - www.ducks-on-the-water.com

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(AdInitialize))]
    public class AdInitializeEditor : CommandEditor
    {
        protected SerializedProperty gameIdiOSProp;
        protected SerializedProperty gameIdAndroidProp;
        protected SerializedProperty testModeProp;

        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            gameIdiOSProp = serializedObject.FindProperty("gameIdiOS");
            gameIdAndroidProp = serializedObject.FindProperty("gameIdAndroid");
            testModeProp = serializedObject.FindProperty("testMode");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(gameIdiOSProp, new GUIContent("ID for iOS", "Id for iOS from Unity Ads Backend."));
            EditorGUILayout.PropertyField(gameIdAndroidProp, new GUIContent("ID for Android", "Id for Android from Unity Ads Backend."));
            EditorGUILayout.PropertyField(testModeProp, new GUIContent("Enable Testmode", "Testmode: video plays are not counted by Unity. Uncheck for live apps."));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
