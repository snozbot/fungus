// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.IO;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(Localization))]
    public class LocalizationEditor : Editor 
    {
        protected SerializedProperty activeLanguageProp;
        protected SerializedProperty localizationFileProp;

        protected virtual void OnEnable()
        {
            activeLanguageProp = serializedObject.FindProperty("activeLanguage");
            localizationFileProp = serializedObject.FindProperty("localizationFile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Localization localization = target as Localization;

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

            serializedObject.ApplyModifiedProperties();
        }

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

        protected virtual void ShowNotification(Localization localization)
        {
            FlowchartWindow.ShowNotification(localization.NotificationText);
            localization.NotificationText = "";
        }
    }
}
