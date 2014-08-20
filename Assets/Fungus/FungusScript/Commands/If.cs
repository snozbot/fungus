using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public enum CompareOperator
	{
		Equals, 				// ==
		NotEquals, 				// !=
		LessThan,				// <
		GreaterThan,			// >
		LessThanOrEquals,		// <=
		GreaterThanOrEquals		// >=
	}

	[HelpText("Execute another sequence IF a condition is true. Sequences can be specified for both true (THEN) and false (ELSE) conditions.")]
	public class If : FungusCommand
	{
		public FungusVariable variable;

		public CompareOperator compareOperator;

		public BooleanData booleanValue;

		public IntegerData integerValue;

		public FloatData floatValue;

		public StringData stringValue;
		
		public Sequence thenSequence;

		public Sequence elseSequence;

		public override void OnEnter()
		{
			bool condition = false;

			if (variable == null)
			{
				Continue();
				return;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				bool lhs = (variable as BooleanVariable).Value;
				bool rhs = booleanValue.Value;

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
				int rhs = integerValue.Value;

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
				float rhs = floatValue.Value;

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
				string rhs = stringValue.Value;

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
				if (thenSequence != null)
				{
					ExecuteSequence(thenSequence);
					return;
				}
			}
			else
			{
				if (elseSequence != null)
				{
					ExecuteSequence(elseSequence);
					return;
				}
			}

			Continue();
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (thenSequence != null)
			{
				connectedSequences.Add(thenSequence);
			}
			if (elseSequence != null)
			{
				connectedSequences.Add(elseSequence);
			}
		}
		
		public override string GetSummary()
		{
			if (variable == null)
			{
				return "Error: No variable selected";
			}

			string summary = variable.key;
			switch (compareOperator)
			{
			case CompareOperator.Equals:
				summary += " == ";
				break;
			case CompareOperator.NotEquals:
				summary += " != ";
				break;
			case CompareOperator.LessThan:
				summary += " < ";
				break;
			case CompareOperator.GreaterThan:
				summary += " > ";
				break;
			case CompareOperator.LessThanOrEquals:
				summary += " <= ";
				break;
			case CompareOperator.GreaterThanOrEquals:
				summary += " >= ";
				break;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				summary += booleanValue.GetDescription();
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				summary += integerValue.GetDescription();
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				summary += floatValue.GetDescription();
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				summary += stringValue.GetDescription();
			}

			summary += " THEN ";

			if (thenSequence == null)
			{
				summary += "<Continue>";
			}
			else
			{
				summary += thenSequence.name;
			}
			summary += " ELSE ";
			if (elseSequence == null)
			{
				summary += "<Continue>";
			}
			else
			{
				summary += elseSequence.name;
			}

			return summary;
		}

		public override bool HasReference(FungusVariable variable)
		{
			return (variable == this.variable);
		}
	}

}