using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Say))]
	public class SayEditor : FungusCommandEditor
	{
		public override void DrawCommandInspectorGUI() 
		{
			Say t = target as Say;

			EditorGUI.BeginChangeCheck();

			string character = EditorGUILayout.TextField(new GUIContent("Character", "Character to display in dialog"), t.character);

			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));
			EditorStyles.textField.wordWrap = true;
			string text = EditorGUILayout.TextArea(t.text, GUILayout.MinHeight(50));

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Say");
				t.character = character;
				t.text = text;
			}
		}
	}

}