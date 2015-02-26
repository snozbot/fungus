using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Reset", 
	             "Resets the state of all commands and local and global variables in the Fungus Script.")]
	[AddComponentMenu("")]
	public class Reset : Command
	{	
		[Tooltip("Reset state of all commands in the script")]
		public bool resetCommands = true;

		[Tooltip("Reset variables back to their default values")]
		public bool resetVariables = true;

		public override void OnEnter()
		{
			GetFungusScript().Reset(resetCommands, resetVariables);
			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}