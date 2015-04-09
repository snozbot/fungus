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

			Language t = target as Language;

			EditorGUILayout.PropertyField(activeLanguageProp);
			EditorGUILayout.PropertyField(localizationFileProp);

			if (GUILayout.Button(new GUIContent("Export Localization File")))
			{
				ExportLocalizationFile(t);
			}

			if (GUILayout.Button(new GUIContent("Export Standard Text")))
			{
				ExportStandardText(t);
			}

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void ExportLocalizationFile(Language language)
		{
			string path = EditorUtility.SaveFilePanel("Export Localization File", "",
			                                          "localization.csv", "");
			if (path.Length == 0) 
			{
				return;
			}

			string csvData = language.GetCSVData();			
			File.WriteAllText(path, csvData);
		}

		public virtual void ExportStandardText(Language language)
		{
			string path = EditorUtility.SaveFilePanel("Export Standard Text", "",
			                                          "standard.txt", "");
			if (path.Length == 0) 
			{
				return;
			}
			
			string textData = language.GetStandardText();			
			File.WriteAllText(path, textData);
		}
	}

}
