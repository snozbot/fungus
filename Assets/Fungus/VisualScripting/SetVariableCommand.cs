using UnityEngine;
using System.Collections;
using Fungus;

public class SetVariableCommand : FungusCommand 
{
	public Variable variable;
	
	public override void OnEnter()
	{
		if (variable.key.Length == 0)
		{
			Finish();
			return;
		}

		Variable v = parentSequenceController.GetVariable(variable.key);
		if (v == null)
		{
			Debug.LogError("Variable " + variable.key + " not defined.");
		}
		else
		{
			if (v.IsSameType(variable))
			{
				v.Assign(variable);
			}
			else
			{
				Debug.LogError("Variables " + variable.key + " and " + v.key + " are not of the same type.");
			}
		}

		Finish();
	}
}
