using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandInfo("Scripting", 
	             "Load Globals", 
	             "Loads a set of global variables previously saved using the SaveGlobals command.", 
	             255, 255, 255)]
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