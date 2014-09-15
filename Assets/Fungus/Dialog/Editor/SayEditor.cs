using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor (typeof(Say))]
	public class SayEditor : CommandEditor
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

		SerializedProperty storyTextProp;
		SerializedProperty characterProp;
		SerializedProperty voiceOverClipProp;
		SerializedProperty showOnceProp;

		void OnEnable()
		{
			storyTextProp = serializedObject.FindProperty("storyText");
			characterProp = serializedObject.FindProperty("character");
			voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
			showOnceProp = serializedObject.FindProperty("showOnce");
		}

		public override void DrawCommandGUI() 
		{
			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();
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

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(storyTextProp);

			Say t = target as Say;

			if (t.character != null &&
			    t.character.profileSprite != null &&
			    t.character.profileSprite.texture != null)
			{
				Texture2D characterTexture = t.character.profileSprite.texture;

				float aspect = (float)characterTexture.width / (float)characterTexture.height;
				Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(50), GUILayout.ExpandWidth(false));
				CharacterEditor characterEditor = Editor.CreateEditor(t.character) as CharacterEditor;
				characterEditor.DrawPreview(previewRect, characterTexture);
				DestroyImmediate(characterEditor);
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			CommandEditor.ObjectField<Character>(characterProp, 
			                                           new GUIContent("Character", "Character to display in dialog"), 
			                                           new GUIContent("<None>"),
													   Character.activeCharacters);

			EditorGUILayout.PropertyField(voiceOverClipProp, 
			                              new GUIContent("Voice Over Clip", "Voice over audio to play when the say text is displayed"));

			EditorGUILayout.PropertyField(showOnceProp, new GUIContent("Show Once", "Show this text once and never show it again."));

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}