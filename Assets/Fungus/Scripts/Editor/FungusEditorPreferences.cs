using UnityEngine;
using UnityEditor;

namespace Fungus
{
    namespace EditorUtils
    {
        /// <summary>
        /// Shows Fungus section in the Edit->Preferences in unity allows you to configure Fungus behaviour
        /// 
        /// ref https://docs.unity3d.com/ScriptReference/PreferenceItem.html
        /// </summary>
        public class FungusEditorPreferences
        {
            // Have we loaded the prefs yet
            private static bool prefsLoaded = false;
            
            public static bool hideMushroomInHierarchy = false;

            // Add preferences section named "My Preferences" to the Preferences Window
            [PreferenceItem("Fungus")]

            public static void PreferencesGUI()
            {
                // Load the preferences
                if (!prefsLoaded)
                {
                    hideMushroomInHierarchy = EditorPrefs.GetBool("hideMushroomInHierarchy", false);
                    prefsLoaded = true;
                }

                // Preferences GUI
                hideMushroomInHierarchy = EditorGUILayout.Toggle("Hide Mushroom Flowchart Icon", hideMushroomInHierarchy);

                // Save the preferences
                if (GUI.changed)
                {
                    EditorPrefs.SetBool("hideMushroomInHierarchy", hideMushroomInHierarchy);
                }
            }
        }
    }
}