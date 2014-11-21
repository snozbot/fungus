using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Stop", 
	             "Stop executing the current Fungus Script.")]
	public class Stop : Command
	{		
		public override void OnEnter()
		{
			Stop();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}