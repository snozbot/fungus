using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public enum JumpCondition
	{
		JumpAlways,
		JumpOnCompare
	}

	public enum CompareOperator
	{
		Equals, 				// ==
		NotEquals, 				// !=
		LessThan,				// <
		GreaterThan,			// >
		LessThanOrEquals,		// <=
		GreaterThanOrEquals		// >=
	}

	public class Jump : FungusCommand
	{
		public JumpCondition jumpCondition;
	
		public Sequence targetSequence; // Only used for Always condition
	
		public string variableKey;

		public CompareOperator compareOperator;

		public BooleanData booleanData;

		public IntegerData integerData;

		public FloatData floatData;

		public StringData stringData;
		
		public Sequence onTrueSequence;

		public Sequence onFalseSequence;

		public override void OnEnter()
		{
			if (jumpCondition == JumpCondition.JumpAlways &&
				targetSequence != null)
			{
				ExecuteSequence(targetSequence);
				return;
			}
		
			Variable v = parentFungusScript.GetVariable(variableKey);
			
			bool condition = false;

			switch (v.type)
			{
			case VariableType.Boolean:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.booleanValue == booleanData.value);
					break;
				case CompareOperator.NotEquals:
				default:
					condition = (v.booleanValue != booleanData.value);
					break;
				}
				break;
			case VariableType.Integer:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.integerValue == integerData.value);
					break;
				case CompareOperator.NotEquals:
					condition = (v.integerValue != integerData.value);
					break;
				case CompareOperator.LessThan:
					condition = (v.integerValue < integerData.value);
					break;
				case CompareOperator.GreaterThan:
					condition = (v.integerValue > integerData.value);
					break;
				case CompareOperator.LessThanOrEquals:
					condition = (v.integerValue <= integerData.value);
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = (v.integerValue >= integerData.value);
					break;
				}
				break;
			case VariableType.Float:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.floatValue == floatData.value);
					break;
				case CompareOperator.NotEquals:
					condition = (v.floatValue != floatData.value);
					break;
				case CompareOperator.LessThan:
					condition = (v.floatValue < floatData.value);
					break;
				case CompareOperator.GreaterThan:
					condition = (v.floatValue > floatData.value);
					break;
				case CompareOperator.LessThanOrEquals:
					condition = (v.floatValue <= floatData.value);
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = (v.floatValue >= floatData.value);
					break;
				}
				break;
			case VariableType.String:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.stringValue == stringData.value);
					break;
				case CompareOperator.NotEquals:
				default:
					condition = (v.stringValue != stringData.value);
					break;
				}
				break;
			}

			if (condition)
			{
				if (onTrueSequence != null)
				{
					ExecuteSequence(onTrueSequence);
					return;
				}
			}
			else
			{
				if (onFalseSequence != null)
				{
					ExecuteSequence(onFalseSequence);
					return;
				}
			}

			Continue();
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (jumpCondition == JumpCondition.JumpAlways &&
			    targetSequence != null)
			{
				connectedSequences.Add(targetSequence);
				return;
			}		
		
			if (onTrueSequence != null)
			{
				connectedSequences.Add(onTrueSequence);
			}
			if (onFalseSequence != null)
			{
				connectedSequences.Add(onFalseSequence);
			}
		}
		
		public override string GetCommandName()
		{
			if (jumpCondition == JumpCondition.JumpAlways)
			{
				return GetType().Name;
			}
			
			return GetType().Name + "?";
		}
	}

}