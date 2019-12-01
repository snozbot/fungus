using System.Linq;
using UnityEditor;
using UnityEngine;

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
            private const string HIDE_MUSH_KEY = "hideMushroomInHierarchy";
            private const string USE_LEGACY_MENUS = "useLegacyMenus";

            public static bool hideMushroomInHierarchy;
            public static bool useLegacyMenus;

            static FungusEditorPreferences()
            {
                LoadOnScriptLoad();
            }

#if UNITY_2019_1_OR_NEWER
            [SettingsProvider]
            public static SettingsProvider CreateFungusSettingsProvider()
            {
                // First parameter is the path in the Settings window.
                // Second parameter is the scope of this setting: it only appears in the Project Settings window.
                var provider = new SettingsProvider("Project/Fungus", SettingsScope.Project)
                {
                    // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                    guiHandler = (searchContext) => PreferencesGUI()

                    // // Populate the search keywords to enable smart search filtering and label highlighting:
                    // keywords = new HashSet<string>(new[] { "Number", "Some String" })
                };

                return provider;
            }

#else

            [PreferenceItem("Fungus")]
#endif
            private static void PreferencesGUI()
            {
                // Load the preferences
                if (!prefsLoaded)
                {
                    LoadOnScriptLoad();
                }

                // Preferences GUI
                hideMushroomInHierarchy = EditorGUILayout.Toggle("Hide Mushroom Flowchart Icon", hideMushroomInHierarchy);
                useLegacyMenus = EditorGUILayout.Toggle(new GUIContent("Legacy Menus", "Force Legacy menus for Event, Add Variable and Add Command menus"), useLegacyMenus);

                EditorGUILayout.Space();
                //ideally if any are null, but typically it is all or nothing that have broken links due to version changes or moving files external to Unity
                if (FungusEditorResources.Add == null)
                {
                    EditorGUILayout.HelpBox("FungusEditorResources need to be regenerated!", MessageType.Error);
                }

                if (GUILayout.Button(new GUIContent("Select Fungus Editor Resources SO", "If Fungus icons are not showing correctly you may need to reassign the references in the FungusEditorResources. Button below will locate it.")))
                {
                    var ids = AssetDatabase.FindAssets("t:FungusEditorResources");
                    if (ids.Length > 0)
                    {
                        var p = AssetDatabase.GUIDToAssetPath(ids[0]);
                        var asset = AssetDatabase.LoadAssetAtPath<FungusEditorResources>(p);
                        Selection.activeObject = asset;
                    }
                    else
                    {
                        Debug.LogError("No FungusEditorResources found!");
                    }
                }

                if (GUILayout.Button("Open Changelog (version info)"))
                {
                    //From project path down, look for our Fungus\Docs\ChangeLog.txt
                    var projectPath = System.IO.Directory.GetParent(Application.dataPath);
                    var fileMacthes = System.IO.Directory.GetFiles(projectPath.FullName, "CHANGELOG.txt", System.IO.SearchOption.AllDirectories);

                    fileMacthes = fileMacthes.Where((x) =>
                    {
                        var fileFolder = System.IO.Directory.GetParent(x);
                        return fileFolder.Name == "Docs" && fileFolder.Parent.Name == "Fungus";
                    }).ToArray();

                    if (fileMacthes == null || fileMacthes.Length == 0)
                    {
                        Debug.LogWarning("Cannot locate Fungus\\Docs\\CHANGELONG.txt");
                    }
                    else
                    {
                        Application.OpenURL(fileMacthes[0]);
                    }
                }

                // Save the preferences
                if (GUI.changed)
                {
                    EditorPrefs.SetBool(HIDE_MUSH_KEY, hideMushroomInHierarchy);
                    EditorPrefs.SetBool(USE_LEGACY_MENUS, useLegacyMenus);
                }
            }

            public static void LoadOnScriptLoad()
            {
                hideMushroomInHierarchy = EditorPrefs.GetBool(HIDE_MUSH_KEY, false);
                useLegacyMenus = EditorPrefs.GetBool(USE_LEGACY_MENUS, false);
                prefsLoaded = true;
            }
        }
    }
}