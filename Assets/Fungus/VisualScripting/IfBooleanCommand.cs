using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class IfBooleanCommand : FungusCommand 
	{
		public string key;

		public Sequence trueSequence;
		public Sequence falseSequence;

		public override void OnExecute()
		{
			if (Variables.GetBoolean(key))
			{
				ExecuteSequence(trueSequence);
			}
			else
			{
				ExecuteSequence(falseSequence);
			}
		}

		public void OnDrawGizmos()
		{
			if (trueSequence == null || falseSequence == null)
			{
				errorMessage = "Please select true and false Sequence objects";
			}
			else
			{
				errorMessage = "";
			}
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (trueSequence != null)
			{
				connectedSequences.Add(trueSequence);
			}
			if (falseSequence != null)
			{
				connectedSequences.Add(falseSequence);
			}
		}
	}
	
}