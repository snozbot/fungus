using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(Choose))]
	public class ChooseEditor : CommandEditor
	{
		static public bool showTagHelp;
		
		protected SerializedProperty chooseTextProp;
		protected SerializedProperty characterProp;
		protected SerializedProperty chooseDialogProp;
		protected SerializedProperty voiceOverClipProp;
		protected SerializedProperty timeoutDurationProp;
		
		protected virtual void OnEnable()
		{
			chooseTextProp = serializedObject.FindProperty("chooseText");
			characterProp = serializedObject.FindProperty("character");
			chooseDialogProp = serializedObject.FindProperty("chooseDialog");
			voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
			timeoutDurationProp = serializedObject.FindProperty("timeoutDuration");
		}
		
		public override void DrawCommandGUI() 
		{
			serializedObject.Update();
			
			
			
			CommandEditor.ObjectField<Character>(characterProp,
			                                     new GUIContent("Character", "Character to display in dialog"), 
			                                     new GUIContent("<None>"),
			                                     Character.activeCharacters);
			
			CommandEditor.ObjectField<ChooseDialog>(chooseDialogProp, 
			                                        new GUIContent("Choose Dialog", "Choose Dialog object to use to display the multiple player choices"), 
			                                        new GUIContent("<Default>"),
			                                        ChooseDialog.activeDialogs);
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.PropertyField(chooseTextProp);
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Tag Help", "Show help info for tags"), new GUIStyle(EditorStyles.miniButton)))
			{
				showTagHelp = !showTagHelp;
			}
			EditorGUILayout.EndHorizontal();
			
			if (showTagHelp)
			{
				SayEditor.DrawTagHelpLabel();
			}
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.PropertyField(voiceOverClipProp, new GUIContent("Voice Over Clip", "Voice over audio to play when the choose text is displayed"));
			
			EditorGUILayout.PropertyField(timeoutDurationProp, new GUIContent("Timeout Duration", "Time limit for player to make a choice. Set to 0 for no limit."));
			
			Choose t = target as Choose;
			
			if (t.character != null &&
			    t.character.profileSprite != null &&
			    t.character.profileSprite.texture != null)
			{
				Texture2D characterTexture = t.character.profileSprite.texture;
				
				float aspect = (float)characterTexture.width / (float)characterTexture.height;
				Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(250), GUILayout.ExpandWidth(true));
				CharacterEditor characterEditor = Editor.CreateEditor(t.character) as CharacterEditor;
				characterEditor.DrawPreview(previewRect, characterTexture);
				DestroyImmediate(characterEditor);
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
	
}