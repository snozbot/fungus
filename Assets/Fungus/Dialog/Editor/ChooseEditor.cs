using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus.Script
{

	[CustomEditor (typeof(Choose))]
	public class ChooseEditor : FungusCommandEditor
	{
		public override void DrawCommandGUI() 
		{
			Choose t = target as Choose;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PrefixLabel(new GUIContent("Choose Text", "Text to display in dialog"));
			GUIStyle sayStyle = new GUIStyle(EditorStyles.textArea);
			sayStyle.wordWrap = true;
			string chooseText = EditorGUILayout.TextArea(t.chooseText, sayStyle, GUILayout.MinHeight(30));

			Character character = FungusCommandEditor.ObjectField<Character>(new GUIContent("Character", "Character to display in dialog"), 
			                                                                 new GUIContent("<None>"),
			                                                                 t.character);

			ChooseDialog dialog = FungusCommandEditor.ObjectField<ChooseDialog>(new GUIContent("Choose Dialog", "Dialog to use when displaying choices"), 
			                                                                    new GUIContent("<Default>"),
			                                                                    t.dialog);

			AudioClip voiceOverClip = EditorGUILayout.ObjectField(new GUIContent("Voice Over Clip", "Voice over audio to play when the choose text is displayed"),
			                                                      t.voiceOverClip,
			                                                      typeof(AudioClip),
			                                                      true) as AudioClip;

			float timeoutDuration = EditorGUILayout.FloatField(new GUIContent("Timeout Duration", "Time limit for player to make a choice. Set to 0 for no limit."), t.timeoutDuration);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Choose");
				t.chooseText = chooseText;
				t.character = character;
				t.dialog = dialog;
				t.voiceOverClip = voiceOverClip;
				t.timeoutDuration = timeoutDuration;
			}			
		}
	}
	
}