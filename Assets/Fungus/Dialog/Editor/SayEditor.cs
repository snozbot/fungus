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

			Character character = FungusCommandEditor.ObjectField<Character>(new GUIContent("Character", "Character to display in dialog"), 
			                                                                 new GUIContent("<None>"),
			                                                                 t.character);

			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));
			GUIStyle sayStyle = new GUIStyle(EditorStyles.textArea);
			sayStyle.wordWrap = true;
			string text = EditorGUILayout.TextArea(t.storyText, sayStyle, GUILayout.MinHeight(30));

			bool displayOnce = EditorGUILayout.Toggle(new GUIContent("Display Once", "Display this text once and never show it again."), t.displayOnce);

			float continueTime = EditorGUILayout.FloatField(new GUIContent("Continue Time", "Time to wait before executing next command in the sequence. Useful for time limited player choices."), t.continueTime);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Say");
				t.character = character;
				t.storyText = text;
				t.displayOnce = displayOnce;
				t.continueTime = continueTime;
			}			
		}
	}

	[CustomPropertyDrawer (typeof(Say.SayOption))]
	public class SayOptionDrawer : PropertyDrawer 
	{	
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty(position, label, property);
			
			SerializedProperty optionTextProp = property.FindPropertyRelative("optionText");
			SerializedProperty targetSequenceProp = property.FindPropertyRelative("targetSequence");
			
			Rect optionTextRect = position;
			optionTextRect.width *= 0.5f;
			Rect targetSequenceRect = position;
			targetSequenceRect.width *= 0.5f;
			targetSequenceRect.x += optionTextRect.width;

			FungusScript fungusScript = FungusScriptWindow.GetFungusScript();

			optionTextProp.stringValue = EditorGUI.TextField(optionTextRect, optionTextProp.stringValue);
			if (fungusScript != null)
			{
				targetSequenceProp.objectReferenceValue = SequenceEditor.SequenceField(targetSequenceRect,
				                                                                       new GUIContent("<Continue>"),
				                                                                       fungusScript,
				                                                                       targetSequenceProp.objectReferenceValue as Sequence);
			}

			EditorGUI.EndProperty();
		}
	}
	
}