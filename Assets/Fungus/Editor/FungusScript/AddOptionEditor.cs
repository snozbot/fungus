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
		public override void DrawCommandGUI() 
		{
			AddOption t = target as AddOption;

			EditorGUI.BeginChangeCheck();

			string optionText = EditorGUILayout.TextField(new GUIContent("Option Text", "Text for option button label"), t.optionText);
			Sequence targetSequence = SequenceEditor.SequenceField(new GUIContent("Target Sequence", "Sequence to execute when option is selected"), t.GetFungusScript(), t.targetSequence);
			AddOption.ShowCondition showCondition = (AddOption.ShowCondition)EditorGUILayout.EnumPopup(new GUIContent("Show Condition", "Condition when this option should be visible."), t.showCondition);

			BooleanVariable booleanVariable = t.booleanVariable;

			if (showCondition == AddOption.ShowCondition.BooleanIsFalse ||
				showCondition == AddOption.ShowCondition.BooleanIsTrue) 
			{
				booleanVariable = FungusVariableEditor.VariableField (new GUIContent ("Boolean Variable", "Boolean variable to test for condition"),
				                                                      t.GetFungusScript (),
				                                                      t.booleanVariable,
				                                                      v => { return v.GetType() == typeof(BooleanVariable); }) as BooleanVariable;
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Option");
				t.optionText = optionText;
				t.targetSequence = targetSequence;
				t.showCondition = showCondition;
				t.booleanVariable = booleanVariable;
			}
		}
	}

}