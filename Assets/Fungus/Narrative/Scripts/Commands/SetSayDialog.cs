using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Narrative", 
	             "Set Say Dialog", 
	             "Sets a custom say dialog to use when displaying story text")]
	[AddComponentMenu("")]
	public class SetSayDialog : Command 
	{
		[Tooltip("The Say Dialog to use for displaying Say story text")]
		public SayDialog sayDialog;

		public override void OnEnter()
		{
			if (sayDialog != null)
			{
				SayDialog.activeSayDialog = sayDialog;
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (sayDialog == null)
			{
				return "Error: No say dialog selected";
			}

			return sayDialog.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}