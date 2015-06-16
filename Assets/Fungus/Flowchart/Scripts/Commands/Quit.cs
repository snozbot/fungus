using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Quit", 
	             "Quits the application. Does not work in Editor or Webplayer builds. Shouldn't generally be used on iOS.")]
	[AddComponentMenu("")]
	public class Quit : Command 
	{
		public override void OnEnter()
		{
			Application.Quit();

			// On platforms that don't support Quit we just continue onto the next command
			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}