using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandCategory("Scripting")]
	[CommandName("Load Globals")]
	[HelpText("Loads a set of global variables previously saved using the SaveGlobals command.")]
	public class LoadGlobals : FungusCommand
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
	}
	
}