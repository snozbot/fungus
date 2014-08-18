using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public enum CallCondition
	{
		CallAlways,
		CallIf
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

	[HelpText("Start running another sequence. Can use a variable comparision to decide which sequence to run.")]
	public class Call : FungusCommand
	{
		public CallCondition callCondition;
	
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
			if (callCondition == CallCondition.CallAlways &&
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
				bool rhs = booleanData.Value;

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
				int rhs = integerData.Value;

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
				float rhs = floatData.Value;

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
				string rhs = stringData.Value;

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
			if (callCondition == CallCondition.CallAlways &&
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
		
		public override string GetSummary()
		{
			if (callCondition == CallCondition.CallAlways)
			{
				if (targetSequence == null)
				{
					return "No target sequence selected";
				}

				return targetSequence.name;
			}
			else
			{
				if (variable == null)
				{
					return "No variable selected";
				}

				string description = "If " + variable.key + " then ";

				if (onTrueSequence == null)
				{
					description += "<Continue>";
				}
				else
				{
					description += onTrueSequence.name;
				}
				description += " else ";
				if (onFalseSequence == null)
				{
					description += "<Continue>";
				}
				else
				{
					description += onFalseSequence.name;
				}

				return description;
			}
		}
	}

}