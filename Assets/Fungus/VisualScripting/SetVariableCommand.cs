using UnityEngine;
using System.Collections;
using Fungus;

public class SetVariableCommand : FungusCommand 
{
	public string variableKey;

	public string stringValue;

	public int integerValue;

	public bool booleanValue;

	public float floatValue;

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
			case VariableType.String:
				v.stringValue = stringValue;
				break;
			case VariableType.Integer:
				v.integerValue = integerValue;
				break;
			case VariableType.Float:
				v.floatValue = floatValue;
				break;
			case VariableType.Boolean:
				v.booleanValue = booleanValue;
				break;
			}
		}

		Finish();
	}
}
