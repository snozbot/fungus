using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(SayCommand))]
	public class SayCommandEditor : FungusCommandEditor
	{
		public override void DrawCommandInspectorGUI() 
		{
			SayCommand t = target as SayCommand;

			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));
			EditorStyles.textField.wordWrap = true;
			string text = EditorGUILayout.TextArea(t.text);
			if (text != t.text)
			{
				Undo.RecordObject(t, "Set Text");
				t.text = text;
			}
		}
	}

}