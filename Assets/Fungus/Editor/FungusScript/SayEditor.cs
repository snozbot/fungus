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

			EditorGUI.BeginChangeCheck();

			string character = EditorGUILayout.TextField(new GUIContent("Character", "Character to display in dialog"), t.character);

			EditorGUILayout.PrefixLabel(new GUIContent("Say Text", "Text to display in dialog"));
			EditorStyles.textField.wordWrap = true;
			string text = EditorGUILayout.TextArea(t.text, GUILayout.MinHeight(50));

			Say.ShowCondition showCondition = (Say.ShowCondition)EditorGUILayout.EnumPopup(new GUIContent("Show Condition", "Condition when this say text should be visible."), t.showCondition);
			
			string booleanVariableKey = t.booleanVariableKey;
			
			if (showCondition == Say.ShowCondition.BooleanIsFalse ||
			    showCondition == Say.ShowCondition.BooleanIsTrue) 
			{
				VariableType type = VariableType.Boolean;
				booleanVariableKey = SequenceEditor.VariableField (new GUIContent ("Boolean Variable", "Boolean variable to test for condition"),
				                                                   t.GetFungusScript (),
				                                                   t.booleanVariableKey,
				                                                   ref type,
				                                                   v => { return v.type == VariableType.Boolean; });						
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Say");
				t.character = character;
				t.text = text;
				t.showCondition = showCondition;
				t.booleanVariableKey = booleanVariableKey;
			}			
			
		}
	}

}