using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(Write))]
	public class WriteEditor : CommandEditor
	{
		static public bool showTagHelp;

		protected SerializedProperty textObjectProp;
		protected SerializedProperty textProp;
		protected SerializedProperty clearTextProp;
		protected SerializedProperty textColorProp;
		protected SerializedProperty setAlphaProp;
		protected SerializedProperty setColorProp;
		protected SerializedProperty waitUntilFinishedProp;

		static public void DrawTagHelpLabel()
		{
			string tagsText = "";
			tagsText += "\n";
			tagsText += TextTagParser.GetTagHelp();

			float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
			EditorGUILayout.SelectableLabel(tagsText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
		}

		protected virtual void OnEnable()
		{
			textObjectProp = serializedObject.FindProperty("textObject");
			textProp = serializedObject.FindProperty("text");
			clearTextProp = serializedObject.FindProperty("clearText");
			textColorProp = serializedObject.FindProperty("textColor");
			setAlphaProp = serializedObject.FindProperty("setAlpha");
			setColorProp = serializedObject.FindProperty("setColor");
			waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(textObjectProp);
			EditorGUILayout.PropertyField(textProp);

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Text Tag Help", "View available tags"), new GUIStyle(EditorStyles.miniButton)))
			{
				showTagHelp = !showTagHelp;
			}
			EditorGUILayout.EndHorizontal();

			if (showTagHelp)
			{
				DrawTagHelpLabel();
			}

			EditorGUILayout.PropertyField(clearTextProp);

			EditorGUILayout.PropertyField(textColorProp);
			switch ((Write.TextColor)textColorProp.enumValueIndex)
			{
			case Write.TextColor.Default:
				break;
			case Write.TextColor.SetVisible:
				break;
			case Write.TextColor.SetAlpha:
				EditorGUILayout.PropertyField(setAlphaProp);
				break;
			case Write.TextColor.SetColor:
				EditorGUILayout.PropertyField(setColorProp);
				break;
			}

			EditorGUILayout.PropertyField(waitUntilFinishedProp);

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}