using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(If))]
	public class IfEditor : FungusCommandEditor 
	{
		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			If t = target as If;

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			FungusVariable fungusVariable = FungusVariableEditor.VariableField(new GUIContent("Variable", "Variable to use in operation"),
			                                                                   t.GetFungusScript(),
			                                                                   t.variable,
			                                                                   null);

			if (fungusVariable != t.variable)
			{
				Undo.RecordObject(t, "Select Variable");
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

			CompareOperator compareOperator = (CompareOperator)EditorGUILayout.Popup(new GUIContent("Compare", 
			                                                                                        "The comparison operator to use when comparing values"), 
			                                                                         (int)t.compareOperator, 
			                                                                         operatorList.ToArray());
			if (compareOperator != t.compareOperator)
			{
				Undo.RecordObject(t, "Select Compare Operator");
				t.compareOperator = compareOperator;
			}

			if (t.variable.GetType() == typeof(BooleanVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("booleanValue"));
			}
			else if (t.variable.GetType() == typeof(IntegerVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("integerValue"));
			}
			else if (t.variable.GetType() == typeof(FloatVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("floatValue"));
			}
			else if (t.variable.GetType() == typeof(StringVariable))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("stringValue"));
			}

			EditorGUILayout.Separator();

			Sequence thenSequence = SequenceEditor.SequenceField(new GUIContent("Then", "Sequence to execute if comparision is true"),
			                                               new GUIContent("<Continue>"),
			                                               t.GetFungusScript(), 
			                                               t.thenSequence);

			Sequence elseSequence = SequenceEditor.SequenceField(new GUIContent("Else", "Sequence to execute if comparision is false"),
			                                                new GUIContent("<Continue>"),
			                                                t.GetFungusScript(), 
			                                                t.elseSequence);

			if (thenSequence != t.thenSequence)
			{
				Undo.RecordObject(t, "Set Then Sequence");
				t.thenSequence = thenSequence;
			}
			if (elseSequence != t.elseSequence)
			{
				Undo.RecordObject(t, "Set Else Sequence");
				t.elseSequence = elseSequence;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

}
