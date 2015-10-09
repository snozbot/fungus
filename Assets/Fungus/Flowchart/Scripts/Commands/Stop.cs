using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Stop", 
	             "Stop executing the Block that contains this command.")]
	[AddComponentMenu("")]
	public class Stop : Command
	{		
		public override void OnEnter()
		{
			StopParentBlock();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}