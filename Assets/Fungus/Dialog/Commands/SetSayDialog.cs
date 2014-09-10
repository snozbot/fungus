using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Set Say Dialog", 
	             "Sets the active dialog to use for displaying story text with the Say command.")]
	public class SetSayDialog : Command 
	{
		public SayDialog sayDialog;	
		static public SayDialog activeDialog;

		public override void OnEnter()
		{
			activeDialog = sayDialog;
			Continue();
		}

		public override string GetSummary()
		{
			if (sayDialog == null)
			{
				return "Error: No dialog selected";
			}

			return sayDialog.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}