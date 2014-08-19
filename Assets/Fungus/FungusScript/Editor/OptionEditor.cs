using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Option))]
	public class OptionEditor : FungusCommandEditor
	{
		public override void DrawCommandGUI() 
		{
			Option t = target as Option;

			EditorGUI.BeginChangeCheck();

			string optionText = EditorGUILayout.TextField(new GUIContent("Option Text", "Text for option button label"), t.optionText);
			Sequence targetSequence = SequenceEditor.SequenceField(new GUIContent("Target Sequence", "Sequence to execute when option is selected"),
			                                                       new GUIContent("<Continue>"),
			                                                       t.GetFungusScript(), 
			                                                       t.targetSequence);
			Option.ShowCondition showCondition = (Option.ShowCondition)EditorGUILayout.EnumPopup(new GUIContent("Show Condition", "Condition when this option should be visible."), t.showCondition);

			BooleanVariable booleanVariable = t.booleanVariable;

			if (showCondition == Option.ShowCondition.BooleanIsFalse ||
				showCondition == Option.ShowCondition.BooleanIsTrue) 
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