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
		static public Dialog dialog;
	
		public class SayOption
		{
			public string optionText;
			public Sequence targetSequence;
		}

		static public List<SayOption> options = new List<SayOption>();

		public Character character;
		public string storyText;
		public bool displayOnce;
		int executionCount;

		public float continueTime;

		public override void OnEnter()
		{
			if (dialog == null)
			{
				Continue();
				return;
			}
			
			if (displayOnce && executionCount > 0)
			{
				Continue();
				return;
			}

			executionCount++;

			dialog.ShowDialog(true);

			dialog.SetCharacter(character);

			if (options.Count > 0)
			{
				List<Dialog.Option> dialogOptions = new List<Dialog.Option>();
				foreach (SayOption sayOption in options)
				{
					Dialog.Option dialogOption = new Dialog.Option();
					dialogOption.text = sayOption.optionText;
					Sequence onSelectSequence = sayOption.targetSequence;

					dialogOption.onSelect = delegate {

						dialog.ShowDialog(false);

						if (onSelectSequence == null)
						{
							Continue ();
						}
						else
						{
							ExecuteSequence(onSelectSequence);
						}
					};

					dialogOptions.Add(dialogOption);
				}
			
				options.Clear();

				dialog.Say(storyText, dialogOptions, 0, delegate {
					dialog.ShowDialog(false);
					Continue();
				});
			}
			else
			{
				dialog.Say(storyText, delegate {
					dialog.ShowDialog(false);
					Continue();
				});
			}
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

		public override void GetConnectedSequences (ref List<Sequence> connectedSequences)
		{
			foreach (SayOption option in options)
			{
				if (option.targetSequence != null)
				{
					connectedSequences.Add(option.targetSequence);
				}
			}
		}
	}

}