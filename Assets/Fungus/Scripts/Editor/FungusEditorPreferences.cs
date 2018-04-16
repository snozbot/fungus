using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Fungus
{
    namespace EditorUtils
    {
        /// <summary>
        /// Shows Fungus section in the Edit->Preferences in unity allows you to configure Fungus behaviour
        /// 
        /// ref https://docs.unity3d.com/ScriptReference/PreferenceItem.html
        /// </summary>
        [InitializeOnLoad]
        public static class FungusEditorPreferences
        {
            // Have we loaded the prefs yet
            private static bool prefsLoaded = false;

            public static bool hideMushroomInHierarchy;

            static FungusEditorPreferences()
            {
                LoadOnScriptLoad();
            }

            // Add preferences section named "My Preferences" to the Preferences Window
            [PreferenceItem("Fungus")]
            public static void PreferencesGUI()
            {
                // Load the preferences
                if (!prefsLoaded)
                {
                    LoadOnScriptLoad();
                }

                // Preferences GUI
                hideMushroomInHierarchy = EditorGUILayout.Toggle("Hide Mushroom Flowchart Icon", hideMushroomInHierarchy);

                // Save the preferences
                if (GUI.changed)
                {
                    EditorPrefs.SetBool("hideMushroomInHierarchy", hideMushroomInHierarchy);
                }
            }

            public static void LoadOnScriptLoad()
            {
                hideMushroomInHierarchy = EditorPrefs.GetBool("hideMushroomInHierarchy", false);
                prefsLoaded = true;
            }
        }
    }
}