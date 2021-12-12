// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if !UNITY_LOCALIZATION
using System.IO;
#endif
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(Localization))]
    public class LocalizationEditor : Editor 
    {
#if !UNITY_LOCALIZATION
        protected SerializedProperty activeLanguageProp;
        protected SerializedProperty localizationFileProp;
#else
        protected SerializedProperty stringTableProp;
        protected SerializedProperty defaultLanguageCodeProp;
        protected SerializedProperty toReplaceProp;
        protected SerializedProperty replaceWithProp;

        protected bool showAdvancedOptions;
#endif
        
        protected virtual void OnEnable()
        {
#if !UNITY_LOCALIZATION
            activeLanguageProp = serializedObject.FindProperty("activeLanguage");
            localizationFileProp = serializedObject.FindProperty("localizationFile");
#else
            stringTableProp = serializedObject.FindProperty("stringTable");
            defaultLanguageCodeProp = serializedObject.FindProperty("defaultLanguageCode");
            toReplaceProp = serializedObject.FindProperty("toReplace");
            replaceWithProp = serializedObject.FindProperty("replaceWith");
#endif
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Localization localization = target as Localization;

            if (localization == null) return;
            
#if !UNITY_LOCALIZATION
            EditorGUILayout.PropertyField(activeLanguageProp);
            EditorGUILayout.PropertyField(localizationFileProp);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Exports a localization csv file to disk. You should save this file in your project assets and then set the Localization File property above to use it.", MessageType.Info);

            if (GUILayout.Button(new GUIContent("Export Localization File")))
            {
                ExportLocalizationFile(localization);
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Exports all standard text in the scene to a text file for easy editing in a text editor. Use the Import option to read the standard text back into the scene.", MessageType.Info);

            if (GUILayout.Button(new GUIContent("Export Standard Text")))
            {
                ExportStandardText(localization);
            }

            if (GUILayout.Button(new GUIContent("Import Standard Text")))
            {
                ImportStandardText(localization);
            }
#else
            EditorGUILayout.PropertyField(stringTableProp);
            EditorGUILayout.PropertyField(defaultLanguageCodeProp);
            
            if (!localization.StringTable.IsEmpty || string.IsNullOrWhiteSpace(stringTableProp.stringValue))
            {
                EditorGUILayout.Space();
                
                if (GUILayout.Button(new GUIContent("Commands -> String Table (Export)")))
                {
                    ExportText(localization);
                }
                
                EditorGUILayout.HelpBox("The Localization Tables window close and reopen on export to refresh the UI.", MessageType.Info);

                if (GUILayout.Button(new GUIContent("String Table  -> Commands (Import)")))
                {
                    ImportText(localization);
                }
                
                EditorGUILayout.Space();
            
                showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Show Advanced Options");
                if (showAdvancedOptions)
                {
                    
                    EditorGUILayout.PropertyField(toReplaceProp, new GUIContent("To Replace"));
                    EditorGUILayout.PropertyField(replaceWithProp, new GUIContent("Replace With"));
                    GUI.enabled = !string.IsNullOrWhiteSpace(toReplaceProp.stringValue) && !string.IsNullOrWhiteSpace(replaceWithProp.stringValue);
                    if (GUILayout.Button(new GUIContent("Find and replace all Keys")))
                    {
                        ReplaceKeyValues(localization);
                        FixLocalizedStrings(localization, false); // fix localized string while we are here because they were broken.
                    }
                    GUI.enabled = true;
                    EditorGUILayout.HelpBox("Find and Replace functionality is useful if you update a Flowchart's Localization ID. 'Fix LocalizedStrings in Fungus Commands' will automatically be run upon completion.", MessageType.Info);

                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button(new GUIContent("Fix LocalizedStrings in Fungus Commands")))
                    {
                        FixLocalizedStrings(localization);
                    }
                }
            }
#endif

            serializedObject.ApplyModifiedProperties();
        }

#if !UNITY_LOCALIZATION
        public virtual void ExportLocalizationFile(Localization localization)
        {
            string path = EditorUtility.SaveFilePanelInProject("Export Localization File",
                                                               "localization.csv",
                                                               "csv",
                                                               "Please enter a filename to save the localization file to");
            if (path.Length == 0) 
            {
                return;
            }

            string csvData = localization.GetCSVData();         
            File.WriteAllText(path, csvData);
            AssetDatabase.ImportAsset(path);

            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            if (textAsset != null)
            {
                localization.LocalizationFile = textAsset;
            }

            ShowNotification(localization);
        }

        public virtual void ExportStandardText(Localization localization)
        {
            string path = EditorUtility.SaveFilePanel("Export Standard Text", "Assets/", "standard.txt", "");
            if (path.Length == 0) 
            {
                return;
            }

            localization.ClearLocalizeableCache();

            string textData = localization.GetStandardText();           
            File.WriteAllText(path, textData);
            AssetDatabase.Refresh();

            ShowNotification(localization);
        }
        
        public virtual void ImportStandardText(Localization localization)
        {
            string path = EditorUtility.OpenFilePanel("Import Standard Text", "Assets/", "txt");
            if (path.Length == 0) 
            {
                return;
            }

            localization.ClearLocalizeableCache();

            string textData = File.ReadAllText(path);
            localization.SetStandardText(textData);

            ShowNotification(localization);
        }
#else
        private void ImportText(Localization localization)
        {
            localization.ImportData();
            ShowNotification(localization);
        }

        private void ExportText(Localization localization)
        {
            localization.ExportData();
            ShowNotification(localization);
        }

        private void FixLocalizedStrings(Localization localization, bool showNotification = true)
        {
            localization.FixLocalizedStrings();
            if (showNotification)
                ShowNotification(localization);
        }
        
        private void ReplaceKeyValues(Localization localization)
        {
            localization.ReplaceKeyValues();
            ShowNotification(localization);
        }
#endif
        
        protected virtual void ShowNotification(Localization localization)
        {
            if (!string.IsNullOrWhiteSpace(localization.NotificationText))
            {
                FlowchartWindow.ShowNotification(localization.NotificationText);
            }
            
            localization.NotificationText = "";
        }
    }
}
