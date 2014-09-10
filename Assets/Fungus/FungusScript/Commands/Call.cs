using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Call", 
	             "Execute another sequence.")]
	public class Call : Command
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
				return "<Continue>";
			}

			return targetSequence.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}