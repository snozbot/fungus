using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(AddOption))]
	public class AddOptionEditor : FungusCommandEditor
	{
		public override void DrawCommandInspectorGUI() 
		{
			AddOption t = target as AddOption;

			EditorGUI.BeginChangeCheck();

			string optionText = EditorGUILayout.TextField(new GUIContent("Option Text", "Text for option button label"), t.optionText);
			Sequence targetSequence = SequenceEditor.SequenceField(new GUIContent("Target Sequence", "Sequence to execute when option is selected"), t.GetFungusScript(), t.targetSequence);
			AddOption.ShowCondition showCondition = (AddOption.ShowCondition)EditorGUILayout.EnumPopup(new GUIContent("Show Condition", "Condition when this option should be visible."), t.showCondition);

			string booleanVariableKey = t.booleanVariableKey;

			if (showCondition == AddOption.ShowCondition.BooleanIsFalse ||
				showCondition == AddOption.ShowCondition.BooleanIsTrue) 
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
				Undo.RecordObject(t, "Set Option");
				t.optionText = optionText;
				t.targetSequence = targetSequence;
				t.showCondition = showCondition;
				t.booleanVariableKey = booleanVariableKey;
			}
		}
	}

}