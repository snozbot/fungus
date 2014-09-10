using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Load Globals", 
	             "Loads a set of global variables previously saved using the SaveGlobals command.")]
	public class LoadGlobals : Command
	{
		public string saveName = "";

		public override void OnEnter()
		{
			GlobalVariables.Load(saveName);
		}
		
		public override string GetSummary()
		{
			return saveName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}
	
}