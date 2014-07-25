using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class ExecuteCommand : FungusCommand 
	{
		public Sequence targetSequence;

		public override void OnExecute()
		{
			if (targetSequence == null)
			{
				return;
			}

			ExecuteSequence(targetSequence);
		}

		public void OnDrawGizmos()
		{
			if (targetSequence == null)
			{
				errorMessage = "Please select a Target Sequence object";
			}
			else
			{
				errorMessage = "";
			}
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (targetSequence != null)
			{
				connectedSequences.Add(targetSequence);
			}
		}
	}
	
}