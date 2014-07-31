using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(CompareCommand))]
	public class CompareCommandEditor : FungusCommandEditor 
	{
		public override void DrawCommandInspectorGUI()
		{
			CompareCommand t = target as CompareCommand;

			FungusScript sc = t.GetParentFungusScript();
			if (sc == null)
			{
				return;
			}

			List<string> keys = new List<string>();
			keys.Add("<None>");
			int index = 0;
			for (int i = 0; i < sc.variables.Count; ++i)
			{
				Variable v = sc.variables[i];
				keys.Add(v.key);
				if (v.key == t.variableKey &&
				    index == 0)
				{
					index = i + 1;
				}
			}

			int newIndex = EditorGUILayout.Popup("Variable", index, keys.ToArray());

			bool keyChanged = (t.variableKey != keys[newIndex]);

			if (keyChanged)
			{
				Undo.RecordObject(t, "Select variable");
				t.variableKey = keys[newIndex];
			}

			if (t.variableKey == "<None>")
			{
				return;
			}

			VariableType variableType = sc.variables[newIndex - 1].type;

			List<GUIContent> operatorList = new List<GUIContent>();
			operatorList.Add(new GUIContent("=="));
			operatorList.Add(new GUIContent("!="));
			if (variableType == VariableType.Integer ||
			    variableType == VariableType.Float)
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

			switch (variableType)
			{
			case VariableType.Boolean:
				booleanValue = EditorGUILayout.Toggle(new GUIContent("Boolean Value", "The boolean value to set the variable with"), booleanValue);
				break;
			case VariableType.Integer:
				integerValue = EditorGUILayout.IntField(new GUIContent("Integer Value", "The integer value to set the variable with"), integerValue);
				break;
			case VariableType.Float:
				floatValue = EditorGUILayout.FloatField(new GUIContent("Float Value", "The float value to set the variable with"), floatValue);
				break;
			case VariableType.String:
				stringValue = EditorGUILayout.TextField(new GUIContent("String Value", "The string value to set the variable with"), stringValue);
				break;
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
			                                               t.GetParentFungusScript(), 
			                                               t.onTrueSequence);

			Sequence onFalse = SequenceEditor.SequenceField(new GUIContent("On False Sequence", "Sequence to execute if comparision is false"),
			                                                t.GetParentFungusScript(), 
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
