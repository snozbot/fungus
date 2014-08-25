using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandCategory("Scripting")]
	[CommandName("Save Globals")]
	[HelpText("Saves all current global variables to be loaded again later using the LoadGlobals command. This provides a basic save game system.")]
	public class SaveGlobals : FungusCommand
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
	}
	
}