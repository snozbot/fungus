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
					"\t{x} Exit\n" +
					"\t{m} Broadcast message\n" +
					"\t{$VarName} Substitute variable";
			
			float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
			EditorGUILayout.SelectableLabel(tagsText, EditorStyles.miniLabel, GUILayout.MinHeight(pixelHeight));
		}

		protected SerializedProperty characterProp;
		protected SerializedProperty sayDialogProp;
		protected SerializedProperty portraitProp;
		protected SerializedProperty storyTextProp;
		protected SerializedProperty voiceOverClipProp;
		protected SerializedProperty showAlwaysProp;
		protected SerializedProperty showCountProp;
		protected SerializedProperty waitForInputProp;

		protected virtual void OnEnable()
		{
			characterProp = serializedObject.FindProperty("character");
			sayDialogProp = serializedObject.FindProperty("sayDialog");
			portraitProp = serializedObject.FindProperty("portrait");
			storyTextProp = serializedObject.FindProperty("storyText");
			voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
			showAlwaysProp = serializedObject.FindProperty("showAlways");
			showCountProp = serializedObject.FindProperty("showCount");
			waitForInputProp = serializedObject.FindProperty("waitForInput");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();

			Say t = target as Say;

			CommandEditor.ObjectField<Character>(characterProp, 
			                                     new GUIContent("Character", "Character to display in dialog"), 
			                                     new GUIContent("<None>"),
			                                     Character.activeCharacters);

			CommandEditor.ObjectField<SayDialog>(sayDialogProp, 
			                                     new GUIContent("Say Dialog", "Say Dialog object to use to display the story text"), 
			                                     new GUIContent("<Default>"),
			                                     SayDialog.activeDialogs);
			bool showPortraits = false;
			// Only show portrait selection if...
			if (t.character != null &&              // Character is selected
			    t.character.portraits != null &&    // Character has a portraits field
			    t.character.portraits.Count > 0 )   // Selected Character has at least 1 portrait
			{
				showPortraits = true;    
			}

			if (showPortraits) 
			{
				CommandEditor.ObjectField<Sprite>(portraitProp, 
			                                	  new GUIContent("Portrait", "Portrait representing speaking character"), 
			                                      new GUIContent("<None>"),
			                                      t.character.portraits);
			}
			else
			{
				t.portrait = null;
			}
			
			EditorGUILayout.PropertyField(storyTextProp);
			
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
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.PropertyField(voiceOverClipProp, 
			                              new GUIContent("Voice Over Clip", "Voice over audio to play when the say text is displayed"));
			
			EditorGUILayout.PropertyField(showAlwaysProp);
			
			if (showAlwaysProp.boolValue == false)
			{
				EditorGUILayout.PropertyField(showCountProp);
			}

			EditorGUILayout.PropertyField(waitForInputProp);

			if (showPortraits && t.portrait != null)
			{
				Texture2D characterTexture = t.portrait.texture;
				
				float aspect = (float)characterTexture.width / (float)characterTexture.height;

				Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));

				CharacterEditor characterEditor = Editor.CreateEditor(t.character) as CharacterEditor;
				characterEditor.DrawPreview(previewRect, characterTexture);
				DestroyImmediate(characterEditor);
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
	
}