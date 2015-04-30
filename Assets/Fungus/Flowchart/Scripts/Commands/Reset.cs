using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Variable", 
	             "Reset", 
	             "Resets the state of all commands and variables in the Flowchart.")]
	[AddComponentMenu("")]
	public class Reset : Command
	{	
		[Tooltip("Reset state of all commands in the script")]
		public bool resetCommands = true;

		[Tooltip("Reset variables back to their default values")]
		public bool resetVariables = true;

		public override void OnEnter()
		{
			GetFlowchart().Reset(resetCommands, resetVariables);
			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}