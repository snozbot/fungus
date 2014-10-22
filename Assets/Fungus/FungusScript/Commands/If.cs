using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
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

	[CommandInfo("Scripting", 
	             "If", 
	             "If the test expression is true, execute the following block of commands.")]
	public class If : Command
	{
		public Variable variable;

		public CompareOperator compareOperator;

		public BooleanData booleanData;

		public IntegerData integerData;

		public FloatData floatData;

		public StringData stringData;
		
		public override void OnEnter()
		{
			if (parentSequence == null)
			{
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
				Continue();
			}
			else
			{
				// Find the next Else or EndIf command at the same indent level as this If command
				bool foundThisCommand = false;
				int indent = indentLevel;
				foreach(Command command in parentSequence.commandList)
				{
					if (foundThisCommand &&
					    command.indentLevel == indent)
					{
						System.Type type = command.GetType();
						if (type == typeof(Else) || 
						    type == typeof(EndIf))
						{
							// Execute command immediately after the Else or EndIf command
							Continue(command);
							return;
						}
					}
					else if (command == this)
					{
						foundThisCommand = true;
					}
				}

				// No matching EndIf command found, so just stop the sequence
				Stop();
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
				summary += booleanData.GetDescription();
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				summary += integerData.GetDescription();
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				summary += floatData.GetDescription();
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				summary += stringData.GetDescription();
			}

			return summary;
		}

		public override bool HasReference(Variable variable)
		{
			return (variable == this.variable);
		}

		public override int GetPostIndent()
		{
			return 1;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}