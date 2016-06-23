/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{

	[CommandInfo("Narrative",
	             "Clear Menu",
	             "Clears the options from a menu dialogue")]
	public class ClearMenu : Command 
	{
		[Tooltip("Menu Dialog to clear the options on")]
		public MenuDialog menuDialog;

		public override void OnEnter()
		{
			menuDialog.Clear();

			Continue();
		}

		public override string GetSummary()
		{
			if (menuDialog == null)
			{
				return "Error: No menu dialog object selected";
			}
			
			return menuDialog.name;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}