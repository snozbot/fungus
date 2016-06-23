/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Variable", 
	             "Set Variable", 
	             "Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.")]
	[AddComponentMenu("")]
	public class SetVariable : Command 
	{
		public enum SetOperator
		{
			Assign,		// =
			Negate,		// =!
			Add, 		// +=
			Subtract,	// -=
			Multiply,	// *=
			Divide		// /=
		}

		[Tooltip("The variable whos value will be set")]
		[VariableProperty(typeof(BooleanVariable),
		                  typeof(IntegerVariable), 
		                  typeof(FloatVariable), 
		                  typeof(StringVariable))]
		public Variable variable;

		[Tooltip("The type of math operation to be performed")]
		public SetOperator setOperator;

		[Tooltip("Boolean value to set with")]
		public BooleanData booleanData;

		[Tooltip("Integer value to set with")]
		public IntegerData integerData;

		[Tooltip("Float value to set with")]
		public FloatData floatData;

		[Tooltip("String value to set with")]
		public StringDataMulti stringData;
		
		public override void OnEnter()
		{
			DoSetOperation();

			Continue();
		}

		public override string GetSummary()
		{
			if (variable == null)
			{
				return "Error: Variable not selected";
			}

			string description = variable.key;

			switch (setOperator)
			{
			default:
			case SetOperator.Assign:
				description += " = ";
				break;
			case SetOperator.Negate:
				description += " =! ";
				break;
			case SetOperator.Add:
				description += " += ";
				break;
			case SetOperator.Subtract:
				description += " -= ";
				break;
			case SetOperator.Multiply:
				description += " *= ";
				break;
			case SetOperator.Divide:
				description += " /= ";
				break;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				description += booleanData.GetDescription();
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				description += integerData.GetDescription();
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				description += floatData.GetDescription();
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				description += stringData.GetDescription();
			}

			return description;
		}

		public override bool HasReference(Variable variable)
		{
			return (variable == this.variable);
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}

		protected virtual void DoSetOperation()
		{
			if (variable == null)
			{
				return;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				BooleanVariable lhs = (variable as BooleanVariable);
				bool rhs = booleanData.Value;
				
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					lhs.value = rhs;
					break;
				case SetOperator.Negate:
					lhs.value = !rhs;
					break;
				}
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				IntegerVariable lhs = (variable as IntegerVariable);
				int rhs = integerData.Value;
				
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					lhs.value = rhs;
					break;
				case SetOperator.Add:
					lhs.value += rhs;
					break;
				case SetOperator.Subtract:
					lhs.value -= rhs;
					break;
				case SetOperator.Multiply:
					lhs.value *= rhs;
					break;
				case SetOperator.Divide:
					lhs.value /= rhs;
					break;
				}
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				FloatVariable lhs = (variable as FloatVariable);
				float rhs = floatData.Value;
				
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					lhs.value = rhs;
					break;
				case SetOperator.Add:
					lhs.value += rhs;
					break;
				case SetOperator.Subtract:
					lhs.value -= rhs;
					break;
				case SetOperator.Multiply:
					lhs.value *= rhs;
					break;
				case SetOperator.Divide:
					lhs.value /= rhs;
					break;
				}
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				StringVariable lhs = (variable as StringVariable);
				string rhs = stringData.Value;
				
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					lhs.value = rhs;
					break;
				}
			}
		}
	}

}
