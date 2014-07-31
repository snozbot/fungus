using UnityEngine;
using System.Collections;
using Fungus;

public class SetVariableCommand : FungusCommand 
{
	public string variableKey;

	public BooleanData booleanData;

	public IntegerData integerData;

	public FloatData floatData;

	public StringData stringData;
	
	public override void OnEnter()
	{
		if (variableKey.Length == 0)
		{
			Finish();
			return;
		}

		Variable v = parentSequenceController.GetVariable(variableKey);
		if (v == null)
		{
			Debug.LogError("Variable " + variableKey + " not defined.");
		}
		else
		{
			switch (v.type)
			{
			case VariableType.Boolean:
				v.booleanValue = booleanData.value;
				break;
			case VariableType.Integer:
				v.integerValue = integerData.value;
				break;
			case VariableType.Float:
				v.floatValue = floatData.value;
				break;
			case VariableType.String:
				v.stringValue = stringData.value;
				break;
			}
		}

		Finish();
	}
}
