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
		[Tooltip("Reset state of all commands in the script")]
		public bool resetCommands = true;

		[Tooltip("Reset local variables back to their default values")]
		public bool resetLocalVariables = true;

		[Tooltip("Reset global variables back to their default values")]
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