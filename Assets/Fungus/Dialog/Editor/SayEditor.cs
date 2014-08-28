using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus.Script
{

	[CustomEditor (typeof(Say))]
	public class SayEditor : FungusCommandEditor
	{
		public override void DrawCommandGUI() 
		{
			Say t = target as Say;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));

			GUIStyle sayStyle = new GUIStyle(EditorStyles.textArea);
			sayStyle.wordWrap = true;

			EditorGUILayout.BeginHorizontal();

			string text = EditorGUILayout.TextArea(t.storyText, sayStyle, GUILayout.MinHeight(40));

			if (t.character != null &&
			    t.character.characterImage != null &&
			    t.character.characterImage.texture != null)
			{
				Texture2D characterTexture = t.character.characterImage.texture;

				float aspect = (float)characterTexture.width / (float)characterTexture.height;
				Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(40), GUILayout.ExpandWidth(false));
				CharacterEditor characterEditor = Editor.CreateEditor(t.character) as CharacterEditor;
				if (characterEditor != null)
				{
					characterEditor.DrawPreview(previewRect, characterTexture);
				}
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			Character character = FungusCommandEditor.ObjectField<Character>(new GUIContent("Character", "Character to display in dialog"), 
			                                                                 new GUIContent("<None>"),
			                                                                 t.character);

			SayDialog dialog = FungusCommandEditor.ObjectField<SayDialog>(new GUIContent("Say Dialog", "Dialog to use when displaying Say command story text"), 
			                                                                  new GUIContent("<Default>"),
			                                                                  t.dialog);

			bool showOnce = EditorGUILayout.Toggle(new GUIContent("Show Once", "Show this text once and never show it again."), t.showOnce);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Say");
				t.storyText = text;
				t.character = character;
				t.dialog = dialog;
				t.showOnce = showOnce;
			}			
		}
	}
	
}