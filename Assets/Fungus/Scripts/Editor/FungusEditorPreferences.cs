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
            const string HIDE_MUSH_KEY = "hideMushroomInHierarchy";
            const string USE_EXP_MENUS = "useExperimentalMenus";

            public static bool hideMushroomInHierarchy;
            public static bool useExperimentalMenus;

            static FungusEditorPreferences()
            {
                LoadOnScriptLoad();
            }

            [SettingsProvider]
            public static SettingsProvider CreateFungusSettingsProvider()
            {
                // First parameter is the path in the Settings window.
                // Second parameter is the scope of this setting: it only appears in the Project Settings window.
                var provider = new SettingsProvider("Project/Fungus", SettingsScope.Project)
                {
                    // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                    guiHandler = (searchContext) =>
                    {
                        // Load the preferences
                        if (!prefsLoaded)
                        {
                            LoadOnScriptLoad();
                        }

                        // Preferences GUI
                        hideMushroomInHierarchy = EditorGUILayout.Toggle("Hide Mushroom Flowchart Icon", hideMushroomInHierarchy);
                        useExperimentalMenus = EditorGUILayout.Toggle(new GUIContent("Experimental Searchable Menus", "Experimental menus replace the Event, Add Variable and Add Command menus with a searchable menu more like the Unity AddComponent menu."), useExperimentalMenus);

                        // Save the preferences
                        if (GUI.changed)
                        {
                            EditorPrefs.SetBool(HIDE_MUSH_KEY, hideMushroomInHierarchy);
                            EditorPrefs.SetBool(USE_EXP_MENUS, useExperimentalMenus);
                        }
                    },

                    // // Populate the search keywords to enable smart search filtering and label highlighting:
                    // keywords = new HashSet<string>(new[] { "Number", "Some String" })
                };

                return provider;
            }

            public static void LoadOnScriptLoad()
            {
                hideMushroomInHierarchy = EditorPrefs.GetBool(HIDE_MUSH_KEY, false);
                useExperimentalMenus = EditorPrefs.GetBool(USE_EXP_MENUS, false);
                prefsLoaded = true;
            }
        }
    }
}