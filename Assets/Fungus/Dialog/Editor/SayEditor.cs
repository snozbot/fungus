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
		static public bool showTagHelp;

		static public void DrawTagHelpLabel()
		{
			string tagsText = "\t{b} Bold Text {/b}\n" + 
				"\t{i} Italic Text {/i}\n" +
					"\t{color=red} Color Text {/color}\n" +
					"\t{w}, {w=0.5} Wait \n" +
					"\t{wi} Wait for input\n" +
					"\t{wc} Wait for input and clear\n" +
					"\t{wp}, {wp=0.5} Wait on punctuation\n" +
					"\t{c} Clear\n" +
					"\t{s}, {s=60} Writing speed (chars per sec)\n" +
					"\t{x} Exit";
			
			float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
			EditorGUILayout.SelectableLabel(tagsText, EditorStyles.miniLabel, GUILayout.MinHeight(pixelHeight));
		}

		public override void DrawCommandGUI() 
		{
			Say t = target as Say;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Tag Help", "Show help info for tags"), new GUIStyle(EditorStyles.miniButton)))
			{
				showTagHelp = !showTagHelp;
			}
			EditorGUILayout.EndHorizontal();

			if (showTagHelp)
			{
				DrawTagHelpLabel();
			}

			GUIStyle sayStyle = new GUIStyle(EditorStyles.textArea);
			sayStyle.wordWrap = true;

			EditorGUILayout.BeginHorizontal();

			string text = EditorGUILayout.TextArea(t.storyText, sayStyle, GUILayout.MinHeight(60));

			if (t.character != null &&
			    t.character.characterImage != null &&
			    t.character.characterImage.texture != null)
			{
				Texture2D characterTexture = t.character.characterImage.texture;

				float aspect = (float)characterTexture.width / (float)characterTexture.height;
				Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(50), GUILayout.ExpandWidth(false));
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
			                                                                 t.character,
			                                                                 Character.activeCharacters);

			SayDialog dialog = FungusCommandEditor.ObjectField<SayDialog>(new GUIContent("Say Dialog", "Dialog to use when displaying Say command story text"), 
			                                                              new GUIContent("<Default>"),
			                                                              t.dialog,
			                                                              SayDialog.activeDialogs);

			AudioClip voiceOverClip = EditorGUILayout.ObjectField(new GUIContent("Voice Over Clip", "Voice over audio to play when the say text is displayed"),
			                                                      t.voiceOverClip,
			                                                      typeof(AudioClip),
			                                                      true) as AudioClip;

			bool showOnce = EditorGUILayout.Toggle(new GUIContent("Show Once", "Show this text once and never show it again."), t.showOnce);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Say");
				t.storyText = text;
				t.character = character;
				t.dialog = dialog;
				t.voiceOverClip = voiceOverClip;
				t.showOnce = showOnce;
			}			
		}
	}
	
}