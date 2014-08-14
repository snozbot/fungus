using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Call))]
	public class CallEditor : FungusCommandEditor 
	{
		public override void DrawCommandGUI()
		{
			serializedObject.Update();
			
			Call t = target as Call;

			FungusScript sc = t.GetFungusScript();
			if (sc == null)
			{
				return;
			}

			CallCondition callCondition = (CallCondition)EditorGUILayout.EnumPopup(new GUIContent("Call Condition", "Condition when call will occur"), t.callCondition);
			if (callCondition != t.callCondition)
			{
				Undo.RecordObject(t, "Set Call Condition");
				t.callCondition = callCondition;
			}
			
			if (t.callCondition == CallCondition.CallAlways)
			{
				Sequence targetSequence = SequenceEditor.SequenceField(new GUIContent("Target Sequence", "Sequence to call"), 
				                                                       new GUIContent("<Continue>"), 
				                                                       t.GetFungusScript(), 
				                                                       t.targetSequence);
				if (targetSequence != t.targetSequence)
				{
					Undo.RecordObject(t, "Set Target Sequence");
					t.targetSequence = targetSequence;
				}
				return;
			}

			FungusVariable fungusVariable = FungusVariableEditor.VariableField(new GUIContent("Variable", "Variable to use in operation"),
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

			if (t.variable.GetType() == typeof(BooleanVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("booleanData"));
			}
			else if (t.variable.GetType() == typeof(IntegerVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("integerData"));
			}
			else if (t.variable.GetType() == typeof(FloatVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("floatData"));
			}
			else if (t.variable.GetType() == typeof(StringVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("stringData"));
			}

			Sequence onTrue = SequenceEditor.SequenceField(new GUIContent("On True Sequence", "Sequence to execute if comparision is true"),
			                                               new GUIContent("<Continue>"),
			                                               t.GetFungusScript(), 
			                                               t.onTrueSequence);

			Sequence onFalse = SequenceEditor.SequenceField(new GUIContent("On False Sequence", "Sequence to execute if comparision is false"),
			                                                new GUIContent("<Continue>"),
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

			serializedObject.ApplyModifiedProperties();
		}
	}

}
