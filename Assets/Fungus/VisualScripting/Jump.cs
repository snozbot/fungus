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
					condition = (v.BooleanValue == booleanData.value);
					break;
				case CompareOperator.NotEquals:
				default:
					condition = (v.BooleanValue != booleanData.value);
					break;
				}
				break;
			case VariableType.Integer:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.IntegerValue == integerData.value);
					break;
				case CompareOperator.NotEquals:
					condition = (v.IntegerValue != integerData.value);
					break;
				case CompareOperator.LessThan:
					condition = (v.IntegerValue < integerData.value);
					break;
				case CompareOperator.GreaterThan:
					condition = (v.IntegerValue > integerData.value);
					break;
				case CompareOperator.LessThanOrEquals:
					condition = (v.IntegerValue <= integerData.value);
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = (v.IntegerValue >= integerData.value);
					break;
				}
				break;
			case VariableType.Float:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.FloatValue == floatData.value);
					break;
				case CompareOperator.NotEquals:
					condition = (v.FloatValue != floatData.value);
					break;
				case CompareOperator.LessThan:
					condition = (v.FloatValue < floatData.value);
					break;
				case CompareOperator.GreaterThan:
					condition = (v.FloatValue > floatData.value);
					break;
				case CompareOperator.LessThanOrEquals:
					condition = (v.FloatValue <= floatData.value);
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = (v.FloatValue >= floatData.value);
					break;
				}
				break;
			case VariableType.String:
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (v.StringValue == stringData.value);
					break;
				case CompareOperator.NotEquals:
				default:
					condition = (v.StringValue != stringData.value);
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