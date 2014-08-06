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
						v.booleanValue = booleanData.value;
						break;
					case SetOperator.Negate:
						v.booleanValue = !booleanData.value;
						break;
					}
					break;
				case VariableType.Integer:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.integerValue = integerData.value;
						break;
					case SetOperator.Negate:
						v.integerValue = -integerData.value;
						break;
					case SetOperator.Add:
						v.integerValue += integerData.value;
						break;
					case SetOperator.Subtract:
						v.integerValue -= integerData.value;
						break;
					case SetOperator.Multiply:
						v.integerValue *= integerData.value;
						break;
					case SetOperator.Divide:
						v.integerValue /= integerData.value;
						break;
					}
					break;
				case VariableType.Float:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.floatValue = floatData.value;
						break;
					case SetOperator.Negate:
						v.floatValue = -floatData.value;
						break;
					case SetOperator.Add:
						v.floatValue += floatData.value;
						break;
					case SetOperator.Subtract:
						v.floatValue -= floatData.value;
						break;
					case SetOperator.Multiply:
						v.floatValue *= floatData.value;
						break;
					case SetOperator.Divide:
						v.floatValue /= floatData.value;
						break;
					}
					break;
				case VariableType.String:
					switch (setOperator)
					{
					default:
					case SetOperator.Assign:
						v.stringValue = stringData.value;
						break;
					}
					break;
				}
			}

			Continue();
		}
	}

}
