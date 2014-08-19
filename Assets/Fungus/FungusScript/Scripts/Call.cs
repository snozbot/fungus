using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[HelpText("Execute another sequence.")]
	public class Call : FungusCommand
	{	
		public Sequence targetSequence;
	
		public override void OnEnter()
		{
			if (targetSequence != null)
			{
				ExecuteSequence(targetSequence);
			}
			else
			{		
				Continue();
			}
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (targetSequence != null)
			{
				connectedSequences.Add(targetSequence);
			}		
		}
		
		public override string GetSummary()
		{
			if (targetSequence == null)
			{
				return "<Continue> (No sequence selected)";
			}

			return targetSequence.name;
		}
	}

}