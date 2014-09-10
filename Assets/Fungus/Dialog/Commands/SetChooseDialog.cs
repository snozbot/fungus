using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Set Choose Dialog", 
	             "Sets the active dialog to use for displaying story text with the Choose command.")]
	public class SetChooseDialog : Command 
	{
		public ChooseDialog chooseDialog;	
		static public ChooseDialog activeDialog;

		public override void OnEnter()
		{
			activeDialog = chooseDialog;
			Continue();
		}

		public override string GetSummary()
		{
			if (chooseDialog == null)
			{
				return "Error: No dialog selected";
			}

			return chooseDialog.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}