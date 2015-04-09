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

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void ExportLocalizationFile(Language language)
		{
			string path = EditorUtility.SaveFilePanel("Export strings", "",
			                                          "localization.csv", "");
			if (path.Length == 0) 
			{
				return;
			}

			Debug.Log(path);

			string csvData = language.ExportLocalizationFile();			
			File.WriteAllText(path, csvData);
		}
	}

}
