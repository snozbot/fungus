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
	
		public FungusVariable variable;

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
		
			bool condition = false;

			if (variable == null)
			{
				Continue();
				return;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (variable as BooleanVariable).Value == booleanData.value;
					break;
				case CompareOperator.NotEquals:
				default:
					condition = (variable as BooleanVariable).Value != booleanData.value;
					break;
				}
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (variable as IntegerVariable).Value == integerData.value;
					break;
				case CompareOperator.NotEquals:
					condition = (variable as IntegerVariable).Value != integerData.value;
					break;
				case CompareOperator.LessThan:
					condition = (variable as IntegerVariable).Value < integerData.value;
					break;
				case CompareOperator.GreaterThan:
					condition = (variable as IntegerVariable).Value > integerData.value;
					break;
				case CompareOperator.LessThanOrEquals:
					condition = (variable as IntegerVariable).Value <= integerData.value;
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = (variable as IntegerVariable).Value >= integerData.value;
					break;
				}
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (variable as FloatVariable).Value == floatData.value;
					break;
				case CompareOperator.NotEquals:
					condition = (variable as FloatVariable).Value != floatData.value;
					break;
				case CompareOperator.LessThan:
					condition = (variable as FloatVariable).Value < floatData.value;
					break;
				case CompareOperator.GreaterThan:
					condition = (variable as FloatVariable).Value > floatData.value;
					break;
				case CompareOperator.LessThanOrEquals:
					condition = (variable as FloatVariable).Value <= floatData.value;
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = (variable as FloatVariable).Value >= floatData.value;
					break;
				}
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = (variable as StringVariable).Value == stringData.value;
					break;
				case CompareOperator.NotEquals:
				default:
					condition = (variable as StringVariable).Value != stringData.value;
					break;
				}
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