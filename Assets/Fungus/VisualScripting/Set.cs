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

		public string variableKey;

		public SetOperator setOperator;

		public BooleanData booleanData;

		public IntegerData integerData;

		public FloatData floatData;

		public StringData stringData;
		
		public override void OnEnter()
		{
			if (variableKey.Length == 0)
			{
				Continue();
				return;
			}

			Variable v = parentFungusScript.GetVariable(variableKey);
			if (v == null)
			{
				Debug.LogError("Variable " + variableKey + " not defined.");
			}
			else
			{
				switch (v.type)
				{
				case VariableType.Boolean:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.BooleanValue = booleanData.value;
						break;
					case SetOperator.Negate:
						v.BooleanValue = !booleanData.value;
						break;
					}
					break;
				case VariableType.Integer:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.IntegerValue = integerData.value;
						break;
					case SetOperator.Negate:
						v.IntegerValue = -integerData.value;
						break;
					case SetOperator.Add:
						v.IntegerValue += integerData.value;
						break;
					case SetOperator.Subtract:
						v.IntegerValue -= integerData.value;
						break;
					case SetOperator.Multiply:
						v.IntegerValue *= integerData.value;
						break;
					case SetOperator.Divide:
						v.IntegerValue /= integerData.value;
						break;
					}
					break;
				case VariableType.Float:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.FloatValue = floatData.value;
						break;
					case SetOperator.Negate:
						v.FloatValue = -floatData.value;
						break;
					case SetOperator.Add:
						v.FloatValue += floatData.value;
						break;
					case SetOperator.Subtract:
						v.FloatValue -= floatData.value;
						break;
					case SetOperator.Multiply:
						v.FloatValue *= floatData.value;
						break;
					case SetOperator.Divide:
						v.FloatValue /= floatData.value;
						break;
					}
					break;
				case VariableType.String:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.StringValue = stringData.value;
						break;
					}
					break;
				}
			}

			Continue();
		}
	}

}
