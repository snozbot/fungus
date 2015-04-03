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

		protected virtual void OnEnable()
		{
			activeLanguageProp = serializedObject.FindProperty("activeLanguage");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Language t = target as Language;

			EditorGUILayout.PropertyField(activeLanguageProp);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("Export Strings File")))
			{
				ExportStrings(t);
			}

			GUILayout.Space(8);

			if (GUILayout.Button(new GUIContent("Import Strings File")))
			{
				ImportStrings(t);
			}

			GUILayout.EndHorizontal();

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

		public virtual void ImportStrings(Language language)
		{
			string path = EditorUtility.OpenFilePanel("Import strings", "", "");
			if (path.Length == 0) 
			{
				return;
			}
			
			string stringsFile = File.ReadAllText(path);			
			language.ImportCSV(stringsFile);
		}
	}

}
