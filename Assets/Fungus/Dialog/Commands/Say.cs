using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandCategory("Dialog")]
	[HelpText("Writes a line of story text to the dialog. A list of options can be specified for the player to choose from. Use a non-zero timeout to give the player a limited time to choose.")]
	public class Say : FungusCommand 
	{
		public SayDialog dialog;
	
		static public SayDialog activeDialog;

		public Character character;
		public string storyText;
		public bool displayOnce;
		int executionCount;

		public override void OnEnter()
		{
			if (displayOnce && executionCount > 0)
			{
				Continue();
				return;
			}
			
			executionCount++;

			// Remember active dialog between Say calls
			if (dialog == null)
			{
				if (activeDialog == null)
				{
					Continue();
					return;
				}
				else
				{
					dialog = activeDialog;
				}
			}
			else
			{
				activeDialog = dialog;
			}
	
			dialog.ShowDialog(true);

			dialog.SetCharacter(character);

			dialog.Say(storyText, delegate {
				dialog.ShowDialog(false);
				Continue();
			});
		}

		public override string GetSummary()
		{
			string summary = "";
			if (character != null)
			{
				summary = character.characterName + ": ";
			}

			summary += "\"" + storyText + "\"";

			return summary;
		}
	}

}