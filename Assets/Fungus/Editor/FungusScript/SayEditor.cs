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

			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));
			EditorStyles.textField.wordWrap = true;
			string text = EditorGUILayout.TextArea(t.text, GUILayout.MinHeight(50));
			if (text != t.text)
			{
				Undo.RecordObject(t, "Set Text");
				t.text = text;
			}
		}
	}

}