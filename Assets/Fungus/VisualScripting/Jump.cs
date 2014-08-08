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

		public FungusVariable compareVariable;

		public bool booleanValue;

		public int integerValue;

		public float floatValue;

		public string stringValue;
		
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
				bool lhs = (variable as BooleanVariable).Value;
				bool rhs = (compareVariable as BooleanVariable) == null ? booleanValue : (compareVariable as BooleanVariable).Value;

				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = lhs == rhs;
					break;
				case CompareOperator.NotEquals:
				default:
					condition = lhs != rhs;
					break;
				}
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				int lhs = (variable as IntegerVariable).Value;
				int rhs = (compareVariable as IntegerVariable) == null ? integerValue : (compareVariable as IntegerVariable).Value;

				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = lhs == rhs;
					break;
				case CompareOperator.NotEquals:
					condition = lhs != rhs;
					break;
				case CompareOperator.LessThan:
					condition = lhs < rhs;
					break;
				case CompareOperator.GreaterThan:
					condition = lhs > rhs;
					break;
				case CompareOperator.LessThanOrEquals:
					condition = lhs <= rhs;
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = lhs >= rhs;
					break;
				}
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				float lhs = (variable as FloatVariable).Value;
				float rhs = (compareVariable as FloatVariable) == null ? floatValue : (compareVariable as FloatVariable).Value;

				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = lhs == rhs;
					break;
				case CompareOperator.NotEquals:
					condition = lhs != rhs;
					break;
				case CompareOperator.LessThan:
					condition = lhs < rhs;
					break;
				case CompareOperator.GreaterThan:
					condition = lhs > rhs;
					break;
				case CompareOperator.LessThanOrEquals:
					condition = lhs <= rhs;
					break;
				case CompareOperator.GreaterThanOrEquals:
					condition = lhs >= rhs;
					break;
				}
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				string lhs = (variable as StringVariable).Value;
				string rhs = (compareVariable as StringVariable) == null ? stringValue : (compareVariable as StringVariable).Value;

				switch (compareOperator)
				{
				case CompareOperator.Equals:
					condition = lhs == rhs;
					break;
				case CompareOperator.NotEquals:
				default:
					condition = lhs != rhs;
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