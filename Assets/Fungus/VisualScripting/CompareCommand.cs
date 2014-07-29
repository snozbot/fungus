using UnityEngine;
using System.Collections;
using Fungus;

public class CompareCommand : FungusCommand
{
	public string variableKey;

	public bool booleanValue;

	public Sequence trueSequence;

	public Sequence falseSequence;

	public override void OnEnter()
	{
		Variable v = parentSequenceController.GetVariable(variableKey);
		if (v != null)
		{
			if (v.booleanValue == booleanValue)
			{
				if (trueSequence != null)
				{
					ExecuteSequence(trueSequence);
					return;
				}
			}
			else
			{
				if (falseSequence != null)
				{
					ExecuteSequence(falseSequence);
					return;
				}
			}
		}

		Finish();
	}
}
