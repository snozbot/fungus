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

			if (GUILayout.Button(new GUIContent("Export to CSV")))
			{
				ExportStrings(t);
			}

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void ExportStrings(Language language)
		{
			string path = EditorUtility.SaveFilePanel("Export strings", "",
			                                          "strings.csv", "");
			if (path.Length == 0) 
			{
				return;
			}
			
			string csvData = language.ExportCSV();			
			File.WriteAllText(path, csvData);
		}
	}

}
