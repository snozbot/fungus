using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Stop", 
	             "Stop executing the current Flowchart.")]
	[AddComponentMenu("")]
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