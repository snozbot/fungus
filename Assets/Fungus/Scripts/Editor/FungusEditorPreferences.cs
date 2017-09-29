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

            public static bool hideMushroomInHierarchy,
                               hideVariableInFlowchartInspector ;

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
                hideVariableInFlowchartInspector = EditorGUILayout.Toggle("Hide Variables in Flowchart Inspector", hideVariableInFlowchartInspector);

                // Save the preferences
                if (GUI.changed)
                {
                    EditorPrefs.SetBool("hideMushroomInHierarchy", hideMushroomInHierarchy);
                    EditorPrefs.SetBool("hideVariableInFlowchartInspector", hideVariableInFlowchartInspector);
                }
            }

            public static void LoadOnScriptLoad()
            {
                hideMushroomInHierarchy = EditorPrefs.GetBool("hideMushroomInHierarchy", false);
                hideVariableInFlowchartInspector = EditorPrefs.GetBool("hideVariableInFlowchartInspector", true);
                prefsLoaded = true;
            }
        }
    }
}