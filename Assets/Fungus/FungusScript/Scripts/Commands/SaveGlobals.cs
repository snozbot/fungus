using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Save Globals", 
	             "Saves all current global variables to persistent storage. These can be loaded back in again in future using the LoadGlobals command. This provides a basic save game system.")]
	public class SaveGlobals : Command
	{
		[Tooltip("Save Name of saved global variable values")]
		public string saveName = "";

		public override void OnEnter()
		{
			GlobalVariables.Save(saveName);
			Continue();
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