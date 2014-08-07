using UnityEngine;
using System.Collections;

namespace Fungus.Script
{

	public class Set : FungusCommand 
	{
		public enum SetOperator
		{
			Assign,		// =
			Negate,		// !
			Add, 		// +
			Subtract,	// -
			Multiply,	// *
			Divide		// /
		}

		public FungusVariable variable;

		public SetOperator setOperator;

		public BooleanData booleanData;

		public IntegerData integerData;

		public FloatData floatData;

		public StringData stringData;
		
		public override void OnEnter()
		{
			if (variable == null)
			{
				Continue();
				return;
			}

			if (variable.GetType() == typeof(BooleanVariable))
			{
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					(variable as BooleanVariable).Value = booleanData.value;
					break;
				case SetOperator.Negate:
					(variable as BooleanVariable).Value = !booleanData.value;
					break;
				}
			}
			else if (variable.GetType() == typeof(IntegerVariable))
			{
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					(variable as IntegerVariable).Value = integerData.value;
					break;
				case SetOperator.Negate:
					(variable as IntegerVariable).Value = -integerData.value;
					break;
				case SetOperator.Add:
					(variable as IntegerVariable).Value += integerData.value;
					break;
				case SetOperator.Subtract:
					(variable as IntegerVariable).Value -= integerData.value;
					break;
				case SetOperator.Multiply:
					(variable as IntegerVariable).Value *= integerData.value;
					break;
				case SetOperator.Divide:
					(variable as IntegerVariable).Value /= integerData.value;
					break;
				}
			}
			else if (variable.GetType() == typeof(FloatVariable))
			{
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					(variable as FloatVariable).Value = floatData.value;
					break;
				case SetOperator.Negate:
					(variable as FloatVariable).Value = -floatData.value;
					break;
				case SetOperator.Add:
					(variable as FloatVariable).Value += floatData.value;
					break;
				case SetOperator.Subtract:
					(variable as FloatVariable).Value -= floatData.value;
					break;
				case SetOperator.Multiply:
					(variable as FloatVariable).Value *= floatData.value;
					break;
				case SetOperator.Divide:
					(variable as FloatVariable).Value /= floatData.value;
					break;
				}
			}
			else if (variable.GetType() == typeof(StringVariable))
			{
				switch (setOperator)
				{
				default:
				case SetOperator.Assign:
					(variable as StringVariable).Value = stringData.value;
					break;
				}
			}

			Continue();
		}
	}

}
