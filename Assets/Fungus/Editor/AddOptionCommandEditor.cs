using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	
	[CustomEditor (typeof(AddOptionCommand))]
	public class AddOptionCommandEditor : FungusCommandEditor
	{
		public override void DrawCommandInspectorGUI() 
		{
			AddOptionCommand t = target as AddOptionCommand;
			
			EditorGUI.BeginChangeCheck();
			
			string newText = EditorGUILayout.TextField(new GUIContent("Text", "Text to display on option button"), t.text);
			Sequence newSequence = SequenceEditor.SequenceField(new GUIContent("Sequence", "Sequence to execute when this option is selected"), 
			                                                    t.GetFungusScript(), 
			                                                    t.sequence);
			AddOptionCommand.Condition newCondition = (AddOptionCommand.Condition)EditorGUILayout.EnumPopup(new GUIContent("Condition", "Conditions for when this option is displayed"), t.condition);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set AddOption command");
				
				t.text = newText;
				t.sequence = newSequence;
				t.condition = newCondition;
			}

			if (t.condition == AddOptionCommand.Condition.ShowOnBoolean || 
			    t.condition == AddOptionCommand.Condition.HideOnBoolean)
			{
				string newBooleanVariableKey = EditorGUILayout.TextField(new GUIContent("Boolean Variable Key", "Boolean variable to check for condition"), t.booleanVariableKey);
				if (newBooleanVariableKey != t.booleanVariableKey)
				{
					Undo.RecordObject(t, "Set Boolean Variable");
					t.booleanVariableKey = newBooleanVariableKey;
				}
			}
		}
	}
	
}