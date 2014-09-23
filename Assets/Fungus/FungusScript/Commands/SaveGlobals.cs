using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Save Globals", 
	             "Saves all current global variables to be loaded again later using the LoadGlobals command. This provides a basic save game system.")]
	public class SaveGlobals : Command
	{
		public string saveName = "";

		public override void OnEnter()
		{
			GlobalVariables.Save(saveName);
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