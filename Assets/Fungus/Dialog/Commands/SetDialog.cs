using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandName("Set Dialog")]
	[CommandCategory("Dialog")]
	[HelpText("Sets the active dialog for displaying story text with the Say command.")]
	public class SetDialog : FungusCommand 
	{
		public Dialog dialogController;

		public override void OnEnter()
		{
			if (dialogController != null)
			{
				Say.dialog = dialogController;
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (dialogController == null)
			{
				return "Error: No dialog selected";
			}

			return dialogController.name;
		}
	}

}