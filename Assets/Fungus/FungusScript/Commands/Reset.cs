using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Reset", 
	             "Resets state of all commands and variables in this Fungus Script.")]
	public class Reset : Command
	{	
		public override void OnEnter()
		{
			GetFungusScript().Reset();
			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}