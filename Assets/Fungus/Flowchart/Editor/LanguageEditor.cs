using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor(typeof(Language))]
	public class LanguageEditor : Editor 
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

			Language language = target as Language;

			EditorGUILayout.PropertyField(activeLanguageProp);
			EditorGUILayout.PropertyField(localizationFileProp);

			if (GUILayout.Button(new GUIContent("Export Localization File")))
			{
				ExportLocalizationFile(language);
			}

			if (GUILayout.Button(new GUIContent("Export Standard Text")))
			{
				ExportStandardText(language);
			}

			if (GUILayout.Button(new GUIContent("Import Standard Text")))
			{
				ImportStandardText(language);
			}

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void ExportLocalizationFile(Language language)
		{
			string path = EditorUtility.SaveFilePanel("Export Localization File", "Assets/",
			                                          "localization.csv", "");
			if (path.Length == 0) 
			{
				return;
			}

			string csvData = language.GetCSVData();			
			File.WriteAllText(path, csvData);
			AssetDatabase.Refresh();
		}

		public virtual void ExportStandardText(Language language)
		{
			string path = EditorUtility.SaveFilePanel("Export Standard Text", "Assets/", "standard.txt", "");
			if (path.Length == 0) 
			{
				return;
			}
			
			string textData = language.GetStandardText();			
			File.WriteAllText(path, textData);
			AssetDatabase.Refresh();
		}

		public virtual void ImportStandardText(Language language)
		{
			string path = EditorUtility.OpenFilePanel("Import Standard Text", "Assets/", "txt");
			if (path.Length == 0) 
			{
				return;
			}

			string textData = File.ReadAllText(path);
			language.SetStandardText(textData);			
		}
	}

}
