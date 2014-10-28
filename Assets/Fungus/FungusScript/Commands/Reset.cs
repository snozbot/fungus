using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Reset", 
	             "Resets the state of all commands and local and global variables in the Fungus Script.")]
	public class Reset : Command
	{	
		public bool resetCommands = true;
		public bool resetLocalVariables = true;
		public bool resetGlobalVariables = true;

		public override void OnEnter()
		{
			GetFungusScript().Reset(resetCommands, resetLocalVariables, resetGlobalVariables);
			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}