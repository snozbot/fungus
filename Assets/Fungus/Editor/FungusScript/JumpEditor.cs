using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Jump))]
	public class JumpEditor : FungusCommandEditor 
	{
		public override void DrawCommandInspectorGUI()
		{
			Jump t = target as Jump;

			FungusScript sc = t.GetFungusScript();
			if (sc == null)
			{
				return;
			}

			JumpCondition jumpCondition = (JumpCondition)EditorGUILayout.EnumPopup(new GUIContent("Jump Condition", "Condition when jump will occur"), t.jumpCondition);
			if (jumpCondition != t.jumpCondition)
			{
				Undo.RecordObject(t, "Set Jump Condition");
				t.jumpCondition = jumpCondition;
			}
			
			if (t.jumpCondition == JumpCondition.JumpAlways)
			{
				Sequence targetSequence = SequenceEditor.SequenceField(new GUIContent("Target Sequence", "Sequence to jump to"), t.GetFungusScript(), t.targetSequence);
				if (targetSequence != t.targetSequence)
				{
					Undo.RecordObject(t, "Set Target Sequence");
					t.targetSequence = targetSequence;
				}
				return;
			}

			FungusVariable fungusVariable = SequenceEditor.VariableField(new GUIContent("Compare Variable", "Variable to use in compare operation"),
			                                                             t.GetFungusScript(),
			                                                             t.variable,
			                                                             null);

			if (fungusVariable != t.variable)
			{
				Undo.RecordObject(t, "Select variable");
				t.variable = fungusVariable;
			}

			if (t.variable == null)
			{
				return;
			}

			List<GUIContent> operatorList = new List<GUIContent>();
			operatorList.Add(new GUIContent("=="));
			operatorList.Add(new GUIContent("!="));
			if (t.variable.GetType() == typeof(IntegerVariable) ||
			    t.variable.GetType() == typeof(FloatVariable))
			{
				operatorList.Add(new GUIContent("<"));
				operatorList.Add(new GUIContent(">"));
				operatorList.Add(new GUIContent("<="));
				operatorList.Add(new GUIContent(">="));
			}

			CompareOperator compareOperator = (CompareOperator)EditorGUILayout.Popup(new GUIContent("Operator", 
			                                                                                        "The comparison operator to use when comparing values"), 
			                                                                         (int)t.compareOperator, 
			                                                                         operatorList.ToArray());
			if (compareOperator != t.compareOperator)
			{
				Undo.RecordObject(t, "Select compare operator");
				t.compareOperator = compareOperator;
			}

			bool booleanValue = t.booleanData.value;
			int integerValue = t.integerData.value;
			float floatValue = t.floatData.value;
			string stringValue = t.stringData.value;

			if (t.variable.GetType() == typeof(BooleanVariable))
			{
				booleanValue = EditorGUILayout.Toggle(new GUIContent("Boolean Value", "The boolean value to set the variable with"), booleanValue);
			}
			else if (t.variable.GetType() == typeof(IntegerVariable))
			{
				integerValue = EditorGUILayout.IntField(new GUIContent("Integer Value", "The integer value to set the variable with"), integerValue);
			}
			else if (t.variable.GetType() == typeof(FloatVariable))
			{
				floatValue = EditorGUILayout.FloatField(new GUIContent("Float Value", "The float value to set the variable with"), floatValue);
			}
			else if (t.variable.GetType() == typeof(StringVariable))
			{
				stringValue = EditorGUILayout.TextField(new GUIContent("String Value", "The string value to set the variable with"), stringValue);
			}

			if (booleanValue != t.booleanData.value)
			{
				Undo.RecordObject(t, "Set boolean value");
				t.booleanData.value = booleanValue;
			}
			else if (integerValue != t.integerData.value)
			{
				Undo.RecordObject(t, "Set integer value");
				t.integerData.value = integerValue;
			}
			else if (floatValue != t.floatData.value)
			{
				Undo.RecordObject(t, "Set float value");
				t.floatData.value = floatValue;
			}
			else if (stringValue != t.stringData.value)
			{
				Undo.RecordObject(t, "Set string value");
				t.stringData.value = stringValue;
			}

			Sequence onTrue = SequenceEditor.SequenceField(new GUIContent("On True Sequence", "Sequence to execute if comparision is true"),
			                                               t.GetFungusScript(), 
			                                               t.onTrueSequence);

			Sequence onFalse = SequenceEditor.SequenceField(new GUIContent("On False Sequence", "Sequence to execute if comparision is false"),
			                                                t.GetFungusScript(), 
			                                                t.onFalseSequence);

			if (onTrue != t.onTrueSequence)
			{
				Undo.RecordObject(t, "Set On True Sequence");
				t.onTrueSequence = onTrue;
			}
			if (onFalse != t.onFalseSequence)
			{
				Undo.RecordObject(t, "Set On False Sequence");
				t.onFalseSequence = onFalse;
			}
		}
	}

}
