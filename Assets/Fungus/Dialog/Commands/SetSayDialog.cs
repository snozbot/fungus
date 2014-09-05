using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandInfo("Dialog", 
	             "Set Say Dialog", 
	             "Sets the active dialog to use for displaying story text with the Say command.")]
	public class SetSayDialog : FungusCommand 
	{
		public SayDialog dialog;	
		static public SayDialog activeDialog;

		public override void OnEnter()
		{
			activeDialog = dialog;
			Continue();
		}

		public override string GetSummary()
		{
			if (dialog == null)
			{
				return "Error: No dialog selected";
			}

			return dialog.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}