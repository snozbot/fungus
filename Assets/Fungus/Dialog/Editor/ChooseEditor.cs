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

		SerializedProperty chooseTextProp;
		SerializedProperty characterProp;
		SerializedProperty voiceOverClipProp;
		SerializedProperty timeoutDurationProp;

		void OnEnable()
		{
			chooseTextProp = serializedObject.FindProperty("chooseText");
			characterProp = serializedObject.FindProperty("character");
			voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
			timeoutDurationProp = serializedObject.FindProperty("timeoutDuration");
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
				SayEditor.DrawTagHelpLabel();
			}

			EditorGUILayout.PropertyField(chooseTextProp);

			CommandEditor.ObjectField<Character>(characterProp,
			                                           new GUIContent("Character", "Character to display in dialog"), 
			                                           new GUIContent("<None>"),
			                                           Character.activeCharacters);

			EditorGUILayout.PropertyField(voiceOverClipProp, new GUIContent("Voice Over Clip", "Voice over audio to play when the choose text is displayed"));

			EditorGUILayout.PropertyField(timeoutDurationProp, new GUIContent("Timeout Duration", "Time limit for player to make a choice. Set to 0 for no limit."));

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}